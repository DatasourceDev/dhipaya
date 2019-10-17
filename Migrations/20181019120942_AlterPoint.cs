using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Package",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "CustomerPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Package",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "CustomerPoints");
        }
    }
}
