﻿// <auto-generated />
using System;
using FurnitureMovement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FurnitureMovement.Migrations
{
    [DbContext(typeof(OrderContext))]
    [Migration("20250330173042_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("FurnitureMovement.Data.Order", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<string>("OrderName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("FurnitureMovement.Data.OrderFurniture", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("AdmissionDate")
                        .HasColumnType("date");

                    b.Property<string>("OrderAuthor")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("OrderID")
                        .HasColumnType("integer");

                    b.Property<long>("OrderQuantity")
                        .HasColumnType("bigint");

                    b.Property<long>("OrderStatus")
                        .HasMaxLength(50)
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("OrderID");

                    b.ToTable("OrderFurnitures");
                });

            modelBuilder.Entity("FurnitureMovement.Data.OrderFurniture", b =>
                {
                    b.HasOne("FurnitureMovement.Data.Order", "Order")
                        .WithMany("Orders")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("FurnitureMovement.Data.Order", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
