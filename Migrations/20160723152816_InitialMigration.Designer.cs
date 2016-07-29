using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using VidsNet.Models;

namespace vidsnet.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20160723152816_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("VidsNet.RealItem", b =>
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

            modelBuilder.Entity("VidsNet.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("VidsNet.SystemMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Message");

                    b.Property<int>("Read");

                    b.Property<int>("Severity");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("SystemMessages");
                });

            modelBuilder.Entity("VidsNet.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Active");

                    b.Property<int>("Level");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VidsNet.UserSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("VidsNet.VirtualItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DeletedTime");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsSeen");

                    b.Property<string>("Name");

                    b.Property<int>("ParentId");

                    b.Property<int>("RealItemId");

                    b.Property<DateTime>("SeenTime");

                    b.Property<int>("Type");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("VirtualItems");
                });
        }
    }
}
