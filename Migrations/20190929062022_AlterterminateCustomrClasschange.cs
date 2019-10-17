using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterterminateCustomrClasschange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminateCustomerClassChanges_Customers_CustomerID",
                table: "TerminateCustomerClassChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminatePointAdjusts_Customers_CustomerID",
                table: "TerminatePointAdjusts");

            migrationBuilder.DropIndex(
                name: "IX_TerminatePointAdjusts_CustomerID",
                table: "TerminatePointAdjusts");

            migrationBuilder.DropIndex(
                name: "IX_TerminateCustomerClassChanges_CustomerID",
                table: "TerminateCustomerClassChanges");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TerminatePointAdjusts_CustomerID",
                table: "TerminatePointAdjusts",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomerClassChanges_CustomerID",
                table: "TerminateCustomerClassChanges",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminateCustomerClassChanges_Customers_CustomerID",
                table: "TerminateCustomerClassChanges",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TerminatePointAdjusts_Customers_CustomerID",
                table: "TerminatePointAdjusts",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
