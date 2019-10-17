using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterAddressEn2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CUR_HouseNo",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_HouseNoEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_Moo",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_MooEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_Soi",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUR_SoiEn",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Aumper",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AumperEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "CustomerImpts",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNo",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseNoEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lane",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LaneEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Moo",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MooEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrefixEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrefixIDEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Province",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Road",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoadEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Soi",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoiEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SurNameEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tumbon",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TumbonEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageName",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNameEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNo",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNoEn",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCodeEn",
                table: "CustomerImpts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CUR_HouseNo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_HouseNoEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_Moo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_MooEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_Soi",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CUR_SoiEn",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "AddressEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Aumper",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "AumperEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Building",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "BuildingEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "HouseNo",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "HouseNoEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Lane",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "LaneEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Moo",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "MooEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "PrefixEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "PrefixIDEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "ProvinceEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Road",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "RoadEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Soi",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "SoiEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "SurNameEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Tumbon",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "TumbonEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageName",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageNameEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageNo",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageNoEn",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "ZipCodeEn",
                table: "CustomerImpts");
        }
    }
}
