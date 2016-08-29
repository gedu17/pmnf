using Microsoft.EntityFrameworkCore;
using VidsNet.Models;
namespace VidsNet.DataModels
{
    public class DatabaseContext : DbContext {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<User> Users {get;set;}
        public DbSet<RealItem> RealItems {get;set;}
        public DbSet<SystemMessage> SystemMessages {get;set;}
        public DbSet<UserSetting> UserSettings {get;set;}
        public DbSet<VirtualItem> VirtualItems {get;set;}

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base (options)  {
            base.Database.EnsureCreated();
        }
    }
}