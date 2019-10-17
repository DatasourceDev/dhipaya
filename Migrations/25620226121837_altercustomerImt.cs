using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class altercustomerImt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aumper",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Building",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "CustomerNo",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "FacebookFlag",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "FacebookID",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "FriendCode",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "IsDhiMember",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Lane",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "LineID",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "RefCode",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Road",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Tumbon",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "UserLevel",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageName",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "VillageNo",
                table: "CustomerImpts");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "CustomerImpts",
                newName: "Passport");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Passport",
                table: "CustomerImpts",
                newName: "ZipCode");

            migrationBuilder.AddColumn<string>(
                name: "Aumper",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNo",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DOB",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookFlag",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookID",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendCode",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsDhiMember",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lane",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineID",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RefCode",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Road",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tumbon",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLevel",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageName",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VillageNo",
                table: "CustomerImpts",
                nullable: true);
        }
    }
}
