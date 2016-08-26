using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vidsnet.Migrations.DatabaseContextSqliteMigrations
{
    public partial class SystemMessage_LongMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LongMessage",
                table: "SystemMessages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongMessage",
                table: "SystemMessages");
        }
    }
}
