using Microsoft.EntityFrameworkCore;
using VidsNet.Models;

namespace VidsNet.DataModels
{
    public class DatabaseContextSqlite : BaseDatabaseContext {

        public new DbSet<VirtualItemSqlite> VirtualItems {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Filename=./data.db");
        }
    }
}