using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMemberClass1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Customers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerClassID",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "CustomerClasses",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnEditable",
                table: "CustomerClasses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerClassID",
                table: "Customers",
                column: "CustomerClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerClasses_CustomerClassID",
                table: "Customers",
                column: "CustomerClassID",
                principalTable: "CustomerClasses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerClasses_CustomerClassID",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerClassID",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerClassID",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "CustomerClasses");

            migrationBuilder.DropColumn(
                name: "UnEditable",
                table: "CustomerClasses");

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                table: "Customers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
