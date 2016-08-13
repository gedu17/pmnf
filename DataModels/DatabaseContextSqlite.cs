using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using VidsNet.DataModels;
using VidsNet.Interfaces;
using VidsNet.Models;
namespace VidsNet.DataModels
{
    public class DatabaseContextSqlite : BaseDatabaseContext {

        public new DbSet<VirtualItemSqlite> VirtualItems {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            //TODO: change data.db location!
            optionsBuilder.UseSqlite("Filename=./data.db");
        }
    }
}