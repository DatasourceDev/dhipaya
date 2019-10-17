using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPrivilgePeriod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "Period",
                table: "Privileges");

            migrationBuilder.RenameColumn(
                name: "LimitedWeek",
                table: "Privileges",
                newName: "PerPrivilegeLimitedWeek");

            migrationBuilder.RenameColumn(
                name: "LimitedMonth",
                table: "Privileges",
                newName: "PerPrivilegeLimitedMonth");

            migrationBuilder.RenameColumn(
                name: "LimitedDay",
                table: "Privileges",
                newName: "PerPrivilegeLimitedDay");

            migrationBuilder.AddColumn<bool>(
                name: "PerPerson",
                table: "Privileges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PerPersonLimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerPersonLimitedDay",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerPersonLimitedMonth",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerPersonPeriod",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerPersoneLimitedWeek",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PerPrivilege",
                table: "Privileges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PerPrivilegeLimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerPrivilegePeriod",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PerPerson",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersonLimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersonLimitedDay",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersonLimitedMonth",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersonPeriod",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersoneLimitedWeek",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPrivilege",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPrivilegeLimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPrivilegePeriod",
                table: "Privileges");

            migrationBuilder.RenameColumn(
                name: "PerPrivilegeLimitedWeek",
                table: "Privileges",
                newName: "LimitedWeek");

            migrationBuilder.RenameColumn(
                name: "PerPrivilegeLimitedMonth",
                table: "Privileges",
                newName: "LimitedMonth");

            migrationBuilder.RenameColumn(
                name: "PerPrivilegeLimitedDay",
                table: "Privileges",
                newName: "LimitedDay");

            migrationBuilder.AddColumn<int>(
                name: "LimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);
        }
    }
}
