using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterIIAOffline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceClass",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutletCode",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyNo",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousPolicyNo",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectCode",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "CustomerPoints",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subclass",
                table: "CustomerPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "InsuranceClass",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "OutletCode",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "PolicyNo",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "PreviousPolicyNo",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "ProjectCode",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "CustomerPoints");

            migrationBuilder.DropColumn(
                name: "Subclass",
                table: "CustomerPoints");
        }
    }
}
