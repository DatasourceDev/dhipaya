using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterRedeemType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Redeems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedeemType",
                table: "Redeems",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Redeems");

            migrationBuilder.DropColumn(
                name: "RedeemType",
                table: "Redeems");
        }
    }
}
