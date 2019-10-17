using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterDoSyn2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IIADoSyned",
                table: "Customers",
                newName: "IIAIgnoreSyned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IIAIgnoreSyned",
                table: "Customers",
                newName: "IIADoSyned");
        }
    }
}
