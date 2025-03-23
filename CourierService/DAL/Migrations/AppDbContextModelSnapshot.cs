﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("Domain.WeatherData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<double?>("AirTemperature")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("OccurrenceTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("WeatherPhenomenon")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<double?>("WindSpeed")
                        .HasColumnType("REAL");

                    b.Property<int?>("WmoCode")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("WeatherDataCollection");
                });
#pragma warning restore 612, 618
        }
    }
}
