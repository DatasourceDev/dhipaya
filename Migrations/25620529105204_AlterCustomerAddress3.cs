using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerAddress3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CUR_AddressEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CUR_AumperEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_BuildingEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CUR_ProvinceEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CUR_TumbonEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_VillageNoEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_ZipCodeEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CUR_AddressEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_AumperEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_BuildingEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_ProvinceEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_TumbonEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_VillageNoEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_ZipCodeEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Customers");
        }
    }
}
