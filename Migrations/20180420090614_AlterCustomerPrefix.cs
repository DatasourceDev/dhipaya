using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrefixEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrefixTh",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrefixEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PrefixTh",
                table: "Customers");
        }
    }
}
