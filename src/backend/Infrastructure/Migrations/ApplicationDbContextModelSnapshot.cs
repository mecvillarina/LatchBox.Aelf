﻿// <auto-generated />
using System;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Domain.Entities.ChainInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BestChainHash")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<long>("BestChainHeight")
                        .HasColumnType("bigint");

                    b.Property<int>("ChainId")
                        .HasColumnType("int");

                    b.Property<string>("ChainIdBase58")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ChainType")
                        .HasMaxLength(16)
                        .HasColumnType("nvarchar(16)");

                    b.Property<string>("Explorer")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("GenesisBlockHash")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("GenesisContractAddress")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLockingFeatureSupported")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTokenCreationFeatureSupported")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVestingFeatureSupported")
                        .HasColumnType("bit");

                    b.Property<string>("LastIrreversibleBlockHash")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<long>("LastIrreversibleBlockHeight")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("LastUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LongestChainHash")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<long>("LongestChainHeight")
                        .HasColumnType("bigint");

                    b.Property<int>("OrderIdx")
                        .HasColumnType("int");

                    b.Property<string>("RpcApi")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("TokenContractAddress")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("ChainInfo", "Chain");
                });

            modelBuilder.Entity("Domain.Entities.TokenInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ChainId")
                        .HasColumnType("int");

                    b.Property<string>("Issuer")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("RawExplorerData")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RawTxData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("Tokens", "Chain");
                });
#pragma warning restore 612, 618
        }
    }
}
