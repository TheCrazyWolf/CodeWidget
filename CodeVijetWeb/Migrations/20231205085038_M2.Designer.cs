﻿// <auto-generated />
using System;
using CodeVijetWeb.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodeVijetWeb.Migrations
{
    [DbContext(typeof(SqlDbContext))]
    [Migration("20231205085038_M2")]
    partial class M2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.14");

            modelBuilder.Entity("CodeVijetWeb.DB.Logs", b =>
                {
                    b.Property<int>("LogsId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LogsId");

                    b.ToTable("Logs");
                });
#pragma warning restore 612, 618
        }
    }
}
