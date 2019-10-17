using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMobilePoint2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "MobilePoints",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseAmt",
                table: "MobilePoints",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "MobilePoints");

            migrationBuilder.DropColumn(
                name: "PurchaseAmt",
                table: "MobilePoints");
        }
    }
}
