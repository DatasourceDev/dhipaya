using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_PointTransacionTypes_TransacionTypeID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TransacionTypeID",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TransacionTypeID",
                table: "Products",
                column: "TransacionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PointTransacionTypes_TransacionTypeID",
                table: "Products",
                column: "TransacionTypeID",
                principalTable: "PointTransacionTypes",
                principalColumn: "TransacionTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
