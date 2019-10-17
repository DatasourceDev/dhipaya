using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitConfigurations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "Configurations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "Configurations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "Configurations",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "Configurations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "Configurations");
        }
    }
}
