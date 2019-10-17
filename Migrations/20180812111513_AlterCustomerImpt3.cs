using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerImpt3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BCryptPwd",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "CustomerImpts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "CustomerImpts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BCryptPwd",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "CustomerImpts");
        }
    }
}
