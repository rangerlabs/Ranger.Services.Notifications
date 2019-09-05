using System;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using PusherServer;
using Ranger.Common;
using Ranger.RabbitMQ;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger<Startup> logger;
        private IContainer container;
        private IBusSubscriber busSubscriber;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {
            this.configuration = configuration;
            this.loggerFactory = loggerFactory;
            this.logger = logger;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                var policy = ScopePolicy.Create("notificationScope");
                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddAuthorization()
                .AddJsonFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddEntityFrameworkNpgsql().AddDbContext<NotificationsDbContext>(options =>
            {
                options.UseNpgsql(configuration["cloudSql:ConnectionString"]);
            },
                ServiceLifetime.Transient
            );

            services.AddTransient<INotificationsDbContextInitializer, NotificationsDbContextInitializer>();

            services.AddSingleton<IEmailNotifier, EmailNotifier>();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://identity:5000/auth";
                    options.ApiName = "notificationsApi";

                    //TODO: Change these to true
                    options.EnableCaching = false;
                    options.RequireHttpsMetadata = false;
                });

            services.AddDataProtection()
                .ProtectKeysWithCertificate(new X509Certificate2(configuration["DataProtectionCertPath:Path"]))
                .PersistKeysToDbContext<NotificationsDbContext>();
            services.AddSingleton<IPusher>(s =>
                        {
                            var options = configuration.GetOptions<RangerPusherOptions>("pusher");
                            return new Pusher(options.AppId, options.Key, options.Secret, new PusherOptions { Cluster = options.Cluster, Encrypted = bool.Parse(options.Encrypted) });
                        });
            services.AddSingleton<IPusherNotifier, PusherNotifier>();
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.AddRabbitMq(loggerFactory);
            container = builder.Build();
            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            this.busSubscriber = app.UseRabbitMQ()
                .SubscribeCommand<SendNewTenantOwnerEmail>((c, e) =>
                   new SendNewTenantOwnerEmailRejected(e.Message, "")
                )
                .SubscribeCommand<SendPusherDomainFrontendNotification>()
                .SubscribeCommand<SendPusherPrivateFrontendNotification>();
        }


        private void OnShutdown()
        {
            this.busSubscriber.Dispose();
        }
    }
}