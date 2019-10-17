using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_No",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "Customer_Type",
                table: "Customers",
                newName: "CustomerNo");

            migrationBuilder.AddColumn<int>(
                name: "CustomerType",
                table: "Customers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerType",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerNo",
                table: "Customers",
                newName: "Customer_Type");

            migrationBuilder.AddColumn<string>(
                name: "Customer_No",
                table: "Customers",
                nullable: true);
        }
    }
}
