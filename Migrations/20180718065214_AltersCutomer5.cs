using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AltersCutomer5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "IsSentSuccess",
            //    table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Success",
                table: "Customers");

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsSentSuccess",
            //    table: "Customers",
            //    nullable: false,
            //    defaultValue: false);
        }
    }
}
