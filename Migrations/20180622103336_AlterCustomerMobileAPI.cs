using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerMobileAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lane",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaneEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Road",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoadEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageName",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNameEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNo",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Lane",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LaneEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Road",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RoadEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VillageName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VillageNameEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VillageNo",
                table: "Customers");
        }
    }
}
