using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPrivilgePeriod3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerPerson",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPrivilege",
                table: "Privileges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PerPerson",
                table: "Privileges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerPrivilege",
                table: "Privileges",
                nullable: false,
                defaultValue: false);
        }
    }
}
