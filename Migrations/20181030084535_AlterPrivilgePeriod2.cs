using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPrivilgePeriod2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerPersoneLimitedWeek",
                table: "Privileges",
                newName: "PerPersonLimitedWeek");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerPersonLimitedWeek",
                table: "Privileges",
                newName: "PerPersoneLimitedWeek");
        }
    }
}
