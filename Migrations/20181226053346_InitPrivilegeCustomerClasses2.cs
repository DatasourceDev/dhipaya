using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitPrivilegeCustomerClasses2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeCustomerClass_Privileges_PrivilegeID",
                table: "PrivilegeCustomerClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeCustomerClass",
                table: "PrivilegeCustomerClass");

            migrationBuilder.RenameTable(
                name: "PrivilegeCustomerClass",
                newName: "PrivilegeCustomerClasses");

            migrationBuilder.RenameIndex(
                name: "IX_PrivilegeCustomerClass_PrivilegeID",
                table: "PrivilegeCustomerClasses",
                newName: "IX_PrivilegeCustomerClasses_PrivilegeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeCustomerClasses",
                table: "PrivilegeCustomerClasses",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeCustomerClasses_Privileges_PrivilegeID",
                table: "PrivilegeCustomerClasses",
                column: "PrivilegeID",
                principalTable: "Privileges",
                principalColumn: "PrivilegeID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeCustomerClasses_Privileges_PrivilegeID",
                table: "PrivilegeCustomerClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeCustomerClasses",
                table: "PrivilegeCustomerClasses");

            migrationBuilder.RenameTable(
                name: "PrivilegeCustomerClasses",
                newName: "PrivilegeCustomerClass");

            migrationBuilder.RenameIndex(
                name: "IX_PrivilegeCustomerClasses_PrivilegeID",
                table: "PrivilegeCustomerClass",
                newName: "IX_PrivilegeCustomerClass_PrivilegeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeCustomerClass",
                table: "PrivilegeCustomerClass",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeCustomerClass_Privileges_PrivilegeID",
                table: "PrivilegeCustomerClass",
                column: "PrivilegeID",
                principalTable: "Privileges",
                principalColumn: "PrivilegeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
