using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterUserRole3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StaffRoleID",
                table: "PageRoles",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "UserRoleID",
                table: "PageRoles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PageRoles_StaffRoleID",
                table: "PageRoles",
                column: "StaffRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_PageRoles_UserRoleID",
                table: "PageRoles",
                column: "UserRoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageRoles_StaffRoles_StaffRoleID",
                table: "PageRoles",
                column: "StaffRoleID",
                principalTable: "StaffRoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRoles_UserRoles_UserRoleID",
                table: "PageRoles",
                column: "UserRoleID",
                principalTable: "UserRoles",
                principalColumn: "UserRoleID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_StaffRoles_StaffRoleID",
                table: "PageRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_UserRoles_UserRoleID",
                table: "PageRoles");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_StaffRoleID",
                table: "PageRoles");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_UserRoleID",
                table: "PageRoles");

            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UserRoleID",
                table: "PageRoles");

            migrationBuilder.AlterColumn<int>(
                name: "StaffRoleID",
                table: "PageRoles",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
