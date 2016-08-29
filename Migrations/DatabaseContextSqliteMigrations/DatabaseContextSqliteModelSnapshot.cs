using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VidsNet.DataModels;

namespace vidsnet.Migrations.DatabaseContextSqliteMigrations
{
    [DbContext(typeof(DatabaseContextSqlite))]
    partial class DatabaseContextSqliteModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("VidsNet.DataModels.BaseVirtualItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DeletedTime");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsViewed");

                    b.Property<string>("Name");

                    b.Property<int>("ParentId");

                    b.Property<int?>("RealItemId");

                    b.Property<int>("Type");

                    b.Property<int>("UserId");

                    b.Property<DateTime>("ViewedTime");

                    b.HasKey("Id");

                    b.ToTable("VirtualItems");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseVirtualItem");
                });

            modelBuilder.Entity("VidsNet.Models.RealItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Extension");

                    b.Property<string>("Name");

                    b.Property<int>("ParentId");

                    b.Property<string>("Path");

                    b.Property<int>("Type");

                    b.Property<int>("UserPathId");

                    b.HasKey("Id");

                    b.ToTable("RealItems");
                });

            modelBuilder.Entity("VidsNet.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("VidsNet.Models.SystemMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LongMessage");

                    b.Property<string>("Message");

                    b.Property<int>("Read");

                    b.Property<int>("Severity");

                    b.Property<DateTime>("Timestamp");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("SystemMessages");
                });

            modelBuilder.Entity("VidsNet.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Active");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("SessionHash");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VidsNet.Models.UserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("VidsNet.Models.VirtualItemSqlite", b =>
                {
                    b.HasBaseType("VidsNet.DataModels.BaseVirtualItem");


                    b.ToTable("VirtualItemSqlite");

                    b.HasDiscriminator().HasValue("VirtualItemSqlite");
                });
        }
    }
}
