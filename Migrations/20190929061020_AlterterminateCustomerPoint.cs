using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterterminateCustomerPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminateCustomerPoints_Customers_CustomerID",
                table: "TerminateCustomerPoints");

            migrationBuilder.DropIndex(
                name: "IX_TerminateCustomerPoints_CustomerID",
                table: "TerminateCustomerPoints");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomerPoints_CustomerID",
                table: "TerminateCustomerPoints",
                column: "CustomerID");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminateCustomerPoints_Customers_CustomerID",
                table: "TerminateCustomerPoints",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
