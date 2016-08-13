using Microsoft.EntityFrameworkCore;
using VidsNet.Models;
namespace VidsNet.DataModels
{
    public abstract class BaseDatabaseContext : DbContext {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<User> Users {get;set;}
        public DbSet<RealItem> RealItems {get;set;}
        public DbSet<SystemMessage> SystemMessages {get;set;}
        public DbSet<UserSetting> UserSettings {get;set;}
        public DbSet<BaseVirtualItem> VirtualItems {get;set;}
    }
}