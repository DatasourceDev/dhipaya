using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMobilePoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "MobilePoints");

            migrationBuilder.AddColumn<int>(
                name: "CustomerID",
                table: "MobilePoints",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "MobilePoints");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "MobilePoints",
                nullable: false,
                defaultValue: "");
        }
    }
}
