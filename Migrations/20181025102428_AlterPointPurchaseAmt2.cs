using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPointPurchaseAmt2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "CustomerPoints");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseAmt",
                table: "CustomerPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseAmt",
                table: "CustomerPoints");

            migrationBuilder.AddColumn<string>(
                name: "Amount",
                table: "CustomerPoints",
                nullable: true);
        }
    }
}
