using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMerchantUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Merchants",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Privileges_CategoryID",
                table: "Privileges",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_MerchantCategories_CategoryID",
                table: "Privileges",
                column: "CategoryID",
                principalTable: "MerchantCategories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_MerchantCategories_CategoryID",
                table: "Privileges");

            migrationBuilder.DropIndex(
                name: "IX_Privileges_CategoryID",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Merchants");
        }
    }
}
