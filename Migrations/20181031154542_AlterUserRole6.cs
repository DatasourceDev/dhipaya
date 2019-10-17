using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterUserRole6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Pages",
                newName: "PageID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_Pages_PageID",
                table: "PageRoles");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_PageID",
                table: "PageRoles");

            migrationBuilder.RenameColumn(
                name: "PageID",
                table: "Pages",
                newName: "ID");
        }
    }
}
