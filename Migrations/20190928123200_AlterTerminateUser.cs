using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterTerminateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TerminateCustomers_Users_UserID",
                table: "TerminateCustomers");

            migrationBuilder.DropIndex(
                name: "IX_TerminateCustomers_UserID",
                table: "TerminateCustomers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TerminateCustomers_UserID",
                table: "TerminateCustomers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TerminateCustomers_Users_UserID",
                table: "TerminateCustomers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
