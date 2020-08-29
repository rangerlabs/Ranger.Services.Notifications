using System.Security.Cryptography.X509Certificates;
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PusherServer;
using Ranger.Common;
using Ranger.InternalHttpClient;
using Ranger.Monitoring.HealthChecks;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;
using Ranger.Services.Operations;

namespace Ranger.Services.Notifications
{
    public class Startup
    {
        private readonly IWebHostEnvironment Environment;
        private readonly IConfiguration configuration;
        private ILoggerFactory loggerFactory;
        private IBusSubscriber busSubscriber;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.Environment = environment;
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.EnableEndpointRouting = false;
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("notificationApi", policyBuilder =>
                    {
                        policyBuilder.RequireScope("notificationApi");
                    });
            });

            var identityAuthority = configuration["httpClient:identityAuthority"];
            services.AddPollyPolicyRegistry();
            services.AddTenantsHttpClient("http://tenants:8082", identityAuthority, "tenantsApi", "cKprgh9wYKWcsm");
            services.AddIdentityHttpClient("http://identity:5000", identityAuthority, "IdentityServerApi", "89pCcXHuDYTXY");

            services.AddDbContext<NotificationsDbContext>(options =>
            {
                options.UseNpgsql(configuration["cloudSql:ConnectionString"]);
            });

            services.AddTransient<INotificationsDbContextInitializer, NotificationsDbContextInitializer>();

            services.AddTransient<IEmailNotifier, EmailNotifier>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://identity:5000/auth";
                    options.ApiName = "notificationsApi";

                    options.RequireHttpsMetadata = false;
                });

            services.AddDataProtection()
                .SetApplicationName("Notifications")
                .ProtectKeysWithCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                .UnprotectKeysWithAnyCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                .PersistKeysToDbContext<NotificationsDbContext>();

            services.AddTransient<IPusher>(s =>
                        {
                            var options = configuration.GetOptions<RangerPusherOptions>("pusher");
                            return new Pusher(options.AppId, options.Key, options.Secret, new PusherOptions { Cluster = options.Cluster, Encrypted = bool.Parse(options.Encrypted) });
                        });
            services.AddTransient<IPusherNotifier, PusherNotifier>();

            services.AddLiveHealthCheck();
            services.AddEntityFrameworkHealthCheck<NotificationsDbContext>();
            services.AddDockerImageTagHealthCheck();
            services.AddRabbitMQHealthCheck();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance<SendGridOptions>(configuration.GetOptions<SendGridOptions>("sendGrid"));
            builder.AddRabbitMq();
        }

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            app.UseRouting();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks();
                endpoints.MapLiveTagHealthCheck();
                endpoints.MapEfCoreTagHealthCheck();
                endpoints.MapDockerImageTagHealthCheck();
                endpoints.MapRabbitMQHealthCheck();
            });
            this.busSubscriber = app.UseRabbitMQ()
                .SubscribeCommand<SendNewPrimaryOwnerEmail>((c, e) =>
                   new SendNewPrimaryOwnerEmailRejected(e.Message, "")
                )
                .SubscribeCommand<SendNewUserEmail>((c, e) =>
                    new SendNewUserEmailRejected(e.Message, "")
                )
                .SubscribeCommand<SendPrimaryOwnerTransferEmails>((c, e) =>
                    new SendPrimaryOwnerTransferEmailsRejected(e.Message, "")
                )
                .SubscribeCommand<SendDomainDeletedEmail>((c, e) =>
                    new SendDomainDeletedEmailRejected(e.Message, ""))
                .SubscribeCommand<SendPrimaryOwnerTransferCancelledEmails>()
                .SubscribeCommand<SendPrimaryOwnerTransferAcceptedEmails>()
                .SubscribeCommand<SendPrimaryOwnerTransferRefusedEmails>()
                .SubscribeCommand<SendResetPasswordEmail>()
                .SubscribeCommand<SendContactFormEmail>()
                .SubscribeCommand<SendChangeEmailEmail>()
                .SubscribeCommand<SendUserPermissionsUpdatedEmail>()
                .SubscribeCommand<SendPusherDomainFrontendNotification>()
                .SubscribeCommand<SendPusherDomainUserPredefinedNotification>()
                .SubscribeCommand<SendPusherDomainUserCustomNotification>()
                .SubscribeCommand<SendPusherDomainCustomNotification>()
                .SubscribeEvent<SubscriptionUpdated>()
                .SubscribeCommand<SendTenantDomainUpdatedEmails>()
                .SubscribeCommand<SendPusherOrganizationDomainUpdatedNotification>();
        }
    }
}