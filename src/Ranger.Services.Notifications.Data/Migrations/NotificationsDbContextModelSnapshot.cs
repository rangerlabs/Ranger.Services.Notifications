﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications.Data.Migrations
{
    [DbContext(typeof(NotificationsDbContext))]
    partial class NotificationsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<string>("FriendlyName")
                        .HasColumnName("friendly_name");

                    b.Property<string>("Xml")
                        .HasColumnName("xml");

                    b.HasKey("Id")
                        .HasName("pk_data_protection_keys");

                    b.ToTable("data_protection_keys");
                });

            modelBuilder.Entity("Ranger.Services.Notifications.Data.FrontendNotification", b =>
                {
                    b.Property<string>("BackendEventKey")
                        .HasColumnName("backend_event_key");

                    b.Property<int>("OperationsState")
                        .HasColumnName("operations_state");

                    b.Property<string>("PusherEventName")
                        .IsRequired()
                        .HasColumnName("pusher_event_name");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnName("text")
                        .HasMaxLength(160);

                    b.HasKey("BackendEventKey", "OperationsState")
                        .HasName("pk_frontend_notifications");

                    b.HasIndex("PusherEventName")
                        .HasName("ix_frontend_notifications_pusher_event_name");

                    b.ToTable("frontend_notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
