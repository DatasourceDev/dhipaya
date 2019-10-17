using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class RemoveFk1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointConditionProducts_Products_ProductID",
                table: "PointConditionProducts");

            migrationBuilder.DropIndex(
                name: "IX_PointConditionProducts_ProductID",
                table: "PointConditionProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PointConditionProducts_ProductID",
                table: "PointConditionProducts",
                column: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditionProducts_Products_ProductID",
                table: "PointConditionProducts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
