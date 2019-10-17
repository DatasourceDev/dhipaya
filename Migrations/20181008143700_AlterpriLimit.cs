using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterpriLimit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewMaxUsage",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "RenewMaxUsagePerPerson",
                table: "Privileges");

            migrationBuilder.RenameColumn(
                name: "MaxUsagePerPerson",
                table: "Privileges",
                newName: "LimitedWeek");

            migrationBuilder.RenameColumn(
                name: "MaxUsage",
                table: "Privileges",
                newName: "LimitedMonth");

            migrationBuilder.AddColumn<int>(
                name: "LimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LimitedDay",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "LimitedDay",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Privileges");

            migrationBuilder.RenameColumn(
                name: "LimitedWeek",
                table: "Privileges",
                newName: "MaxUsagePerPerson");

            migrationBuilder.RenameColumn(
                name: "LimitedMonth",
                table: "Privileges",
                newName: "MaxUsage");

            migrationBuilder.AddColumn<string>(
                name: "RenewMaxUsage",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenewMaxUsagePerPerson",
                table: "Privileges",
                nullable: true);
        }
    }
}
