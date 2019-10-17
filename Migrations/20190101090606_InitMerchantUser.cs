using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitMerchantUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantID",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Merchants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_UserID",
                table: "Merchants",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Merchants_Users_UserID",
                table: "Merchants",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Merchants_Users_UserID",
                table: "Merchants");

            migrationBuilder.DropIndex(
                name: "IX_Merchants_UserID",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Merchants");

            migrationBuilder.AddColumn<int>(
                name: "MerchantID",
                table: "Users",
                nullable: true);
        }
    }
}
