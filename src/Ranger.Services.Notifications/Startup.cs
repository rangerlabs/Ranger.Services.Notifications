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
            // Workaround for MAC validation issues on MacOS
            if (configuration.IsIntegrationTesting())
            {
                services.AddDataProtection()
                   .SetApplicationName("Notifications")
                   .PersistKeysToDbContext<NotificationsDbContext>();
            }
            else
            {
                services.AddDataProtection()
                    .SetApplicationName("Notifications")
                    .ProtectKeysWithCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                    .UnprotectKeysWithAnyCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                    .PersistKeysToDbContext<NotificationsDbContext>();
            }

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

        public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
        {
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
            app.UseRabbitMQ()
                .SubscribeCommandWithHandler<SendNewPrimaryOwnerEmail>((c, e) =>
                   new SendNewPrimaryOwnerEmailRejected(e.Message, "")
                )
                .SubscribeCommandWithHandler<SendNewUserEmail>((c, e) =>
                    new SendNewUserEmailRejected(e.Message, "")
                )
                .SubscribeCommandWithHandler<SendPrimaryOwnerTransferEmails>((c, e) =>
                    new SendPrimaryOwnerTransferEmailsRejected(e.Message, "")
                )
                .SubscribeCommandWithHandler<SendDomainDeletedEmail>((c, e) =>
                    new SendDomainDeletedEmailRejected(e.Message, ""))
                .SubscribeCommandWithHandler<SendPrimaryOwnerTransferCancelledEmails>()
                .SubscribeCommandWithHandler<SendPrimaryOwnerTransferAcceptedEmails>()
                .SubscribeCommandWithHandler<SendPrimaryOwnerTransferRefusedEmails>()
                .SubscribeCommandWithHandler<SendResetPasswordEmail>()
                .SubscribeCommandWithHandler<SendContactFormEmail>()
                .SubscribeCommandWithHandler<SendChangeEmailEmail>()
                .SubscribeCommandWithHandler<SendUserPermissionsUpdatedEmail>()
                .SubscribeCommandWithHandler<SendPusherDomainFrontendNotification>()
                .SubscribeCommandWithHandler<SendPusherDomainUserPredefinedNotification>()
                .SubscribeCommandWithHandler<SendPusherDomainUserCustomNotification>()
                .SubscribeCommandWithHandler<SendPusherDomainCustomNotification>()
                .SubscribeCommandWithHandler<SendTenantDomainUpdatedEmails>()
                .SubscribeCommandWithHandler<SendPusherOrganizationDomainUpdatedNotification>()
                .SubscribeEventWithHandler<SubscriptionUpdated>();
        }
    }
}