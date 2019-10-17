using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterUserRole7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "PageRoles");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "PageRoles");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "PageRoles");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "PageRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "PageRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "PageRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "PageRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "PageRoles",
                nullable: true);
        }
    }
}
