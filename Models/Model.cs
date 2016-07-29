using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using VidsNet.Models;
namespace VidsNet.Models
{
    public class DatabaseContext : DbContext {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<User> Users {get;set;}
        public DbSet<RealItem> RealItems {get;set;}
        public DbSet<SystemMessage> SystemMessages {get;set;}
        public DbSet<UserSetting> UserSettings {get;set;}
        public DbSet<VirtualItem> VirtualItems {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //TODO: change data.db location!
            optionsBuilder.UseSqlite("Filename=./data.db");
        }
    }
}