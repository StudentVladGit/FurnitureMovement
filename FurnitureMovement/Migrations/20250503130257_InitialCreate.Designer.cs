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
    [Migration("20250503130257_InitialCreate")]
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

            modelBuilder.Entity("FurnitureMovement.Data.Furniture", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<int>("FurnitureNameID")
                        .HasColumnType("integer");

                    b.Property<int>("OrderID")
                        .HasColumnType("integer");

                    b.Property<long>("OrderQuantity")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("FurnitureNameID");

                    b.HasIndex("OrderID");

                    b.ToTable("Furnitures", (string)null);
                });

            modelBuilder.Entity("FurnitureMovement.Data.FurnitureName", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<int>("DeleteIndicator")
                        .HasColumnType("integer");

                    b.Property<string>("Drawing")
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .HasColumnType("text");

                    b.Property<string>("Material")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("ProductionTime")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("FurnitureNames", (string)null);
                });

            modelBuilder.Entity("FurnitureMovement.Data.Order", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("AdmissionDate")
                        .HasColumnType("date");

                    b.Property<int>("DeleteIndicator")
                        .HasColumnType("integer");

                    b.Property<int>("OrderAuthorID")
                        .HasColumnType("integer");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<long>("OrderPriority")
                        .HasMaxLength(50)
                        .HasColumnType("bigint");

                    b.Property<long>("OrderStatus")
                        .HasMaxLength(50)
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("OrderAuthorID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("FurnitureMovement.Data.OrderAuthor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<int>("DeleteIndicator")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("OrderAuthors", (string)null);
                });

            modelBuilder.Entity("FurnitureMovement.Data.WarehouseItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("ID"));

                    b.Property<DateTime>("AdmissionDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Drawing")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FurnitureName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FurnitureNameId")
                        .HasColumnType("integer");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Material")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Quantity")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.ToTable("WarehouseItems", (string)null);
                });

            modelBuilder.Entity("FurnitureMovement.Data.Furniture", b =>
                {
                    b.HasOne("FurnitureMovement.Data.FurnitureName", "FurnitureName")
                        .WithMany()
                        .HasForeignKey("FurnitureNameID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FurnitureMovement.Data.Order", "Order")
                        .WithMany("Furnitures")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FurnitureName");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("FurnitureMovement.Data.Order", b =>
                {
                    b.HasOne("FurnitureMovement.Data.OrderAuthor", "OrderAuthor")
                        .WithMany()
                        .HasForeignKey("OrderAuthorID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OrderAuthor");
                });

            modelBuilder.Entity("FurnitureMovement.Data.Order", b =>
                {
                    b.Navigation("Furnitures");
                });
#pragma warning restore 612, 618
        }
    }
}
