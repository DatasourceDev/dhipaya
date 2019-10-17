using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPrivilege : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RenewMaxUsage",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenewMaxUsagePerPerson",
                table: "Privileges",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewMaxUsage",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "RenewMaxUsagePerPerson",
                table: "Privileges");
        }
    }
}
