﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ranger.Services.Notifications.Data;

namespace Ranger.Services.Notifications.Data.Migrations
{
    [DbContext(typeof(NotificationsDbContext))]
    [Migration("20191218031332_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("FriendlyName")
                        .HasColumnName("friendly_name")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnName("xml")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_data_protection_keys");

                    b.ToTable("data_protection_keys");
                });

            modelBuilder.Entity("Ranger.Services.Notifications.Data.FrontendNotification", b =>
                {
                    b.Property<string>("BackendEventKey")
                        .HasColumnName("backend_event_key")
                        .HasColumnType("text");

                    b.Property<int>("OperationsState")
                        .HasColumnName("operations_state")
                        .HasColumnType("integer");

                    b.Property<string>("PusherEventName")
                        .IsRequired()
                        .HasColumnName("pusher_event_name")
                        .HasColumnType("text");

                    b.Property<string>("Text")
                        .HasColumnName("text")
                        .HasColumnType("character varying(160)")
                        .HasMaxLength(160);

                    b.HasKey("BackendEventKey", "OperationsState");

                    b.HasIndex("PusherEventName");

                    b.ToTable("frontend_notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
