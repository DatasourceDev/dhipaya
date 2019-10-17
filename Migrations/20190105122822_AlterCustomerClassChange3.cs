using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerClassChange3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Update_On",
                table: "CustomerClassChanges",
                newName: "Create_On");

            migrationBuilder.RenameColumn(
                name: "Update_By",
                table: "CustomerClassChanges",
                newName: "Create_By");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClassChanges_CustomerID",
                table: "CustomerClassChanges",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerClassChanges_Customers_CustomerID",
                table: "CustomerClassChanges",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerClassChanges_Customers_CustomerID",
                table: "CustomerClassChanges");

            migrationBuilder.DropIndex(
                name: "IX_CustomerClassChanges_CustomerID",
                table: "CustomerClassChanges");

            migrationBuilder.RenameColumn(
                name: "Create_On",
                table: "CustomerClassChanges",
                newName: "Update_On");

            migrationBuilder.RenameColumn(
                name: "Create_By",
                table: "CustomerClassChanges",
                newName: "Update_By");
        }
    }
}
