﻿// <auto-generated />
using System;
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
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
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

            modelBuilder.Entity("Ranger.RabbitMQ.OutboxMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("InsertedAt")
                        .HasColumnName("inserted_at")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MessageId")
                        .HasColumnName("message_id")
                        .HasColumnType("integer");

                    b.Property<bool>("Nacked")
                        .HasColumnName("nacked")
                        .HasColumnType("boolean");

                    b.HasKey("Id")
                        .HasName("pk_outbox");

                    b.HasIndex("MessageId")
                        .HasName("ix_outbox_message_id");

                    b.ToTable("outbox");
                });

            modelBuilder.Entity("Ranger.RabbitMQ.RangerRabbitMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnName("body")
                        .HasColumnType("text");

                    b.Property<string>("Headers")
                        .IsRequired()
                        .HasColumnName("headers")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pk_ranger_rabbit_message");

                    b.ToTable("ranger_rabbit_message");
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

            modelBuilder.Entity("Ranger.RabbitMQ.OutboxMessage", b =>
                {
                    b.HasOne("Ranger.RabbitMQ.RangerRabbitMessage", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .HasConstraintName("fk_outbox_ranger_rabbit_message_message_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
