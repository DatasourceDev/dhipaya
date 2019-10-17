using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class Alteraddressen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TumbonNameEn",
                table: "Tumbons",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceNameEn",
                table: "Provinces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AumphurNameEn",
                table: "Aumphurs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TumbonNameEn",
                table: "Tumbons");

            migrationBuilder.DropColumn(
                name: "ProvinceNameEn",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "AumphurNameEn",
                table: "Aumphurs");
        }
    }
}
