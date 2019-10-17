using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPageRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_Pages_PageID",
                table: "PageRoles");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_PageID",
                table: "PageRoles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PageRoles_PageID",
                table: "PageRoles",
                column: "PageID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageRoles_Pages_PageID",
                table: "PageRoles",
                column: "PageID",
                principalTable: "Pages",
                principalColumn: "PageID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
