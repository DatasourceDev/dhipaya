using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMobilePoint3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IDCard",
                table: "MobilePoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport",
                table: "MobilePoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Passport",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDCard",
                table: "MobilePoints");

            migrationBuilder.DropColumn(
                name: "Passport",
                table: "MobilePoints");

            migrationBuilder.DropColumn(
                name: "Passport",
                table: "Customers");
        }
    }
}
