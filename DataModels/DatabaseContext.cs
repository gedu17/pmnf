using Microsoft.EntityFrameworkCore;
using VidsNet.Models;

namespace VidsNet.DataModels
{
    public class DatabaseContext : BaseDatabaseContext {
        public new DbSet<VirtualItem> VirtualItems {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql(@"Server=localhost;database=vidsnet;uid=vidsnet;pwd=aEB0CytkQkfsOjp8eWod;");
        }
    }
}