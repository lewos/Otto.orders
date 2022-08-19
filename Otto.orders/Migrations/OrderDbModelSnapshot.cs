﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Otto.orders.Models;

#nullable disable

namespace Otto.orders.Migrations
{
    [DbContext(typeof(OrderDb))]
    partial class OrderDbModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Otto.orders.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<long?>("BusinessId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool?>("InProgress")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("InProgressDateTimeModified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("InProgressDateTimeTaken")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ItemDescription")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("MOrderId")
                        .HasColumnType("bigint");

                    b.Property<long?>("MUserId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("Modified")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PackId")
                        .HasColumnType("text");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("SKU")
                        .HasColumnType("text");

                    b.Property<int>("ShippingStatus")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("UserIdInProgress")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("MOrderId")
                        .IsUnique();

                    b.ToTable("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
