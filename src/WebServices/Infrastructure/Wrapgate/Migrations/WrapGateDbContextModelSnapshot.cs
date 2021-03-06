﻿// <auto-generated />
using Aiursoft.Wrapgate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using System;

namespace Aiursoft.Wrapgate.Migrations
{
    [DbContext(typeof(WrapgateDbContext))]
    public class WrapgateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("RecordUniqueName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapgateApp", b =>
                {
                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AppId");

                    b.ToTable("WrapApps");
                });

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapRecord", b =>
                {
                    b.HasOne("Aiursoft.Wrapgate.SDK.Models.WrapgateApp", "App")
                        .WithMany("WrapRecords")
                        .HasForeignKey("AppId");
                });
#pragma warning restore 612, 618
        }
    }
}
