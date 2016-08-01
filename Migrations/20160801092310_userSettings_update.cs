using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vidsnet.Migrations
{
    public partial class userSettings_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("description", "UserSettings", "string", true,
            null, false, null, false, "No description");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("description", "UserSettings");
        }
    }
}
