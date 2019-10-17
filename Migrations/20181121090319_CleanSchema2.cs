using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class CleanSchema2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_PrivilegeType_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropTable(
                name: "PrivilegeType");

            migrationBuilder.DropIndex(
                name: "IX_Privileges_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "StaffRoleID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "GoldLady",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPersonLimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PerPrivilegeLimitType",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "Main",
                table: "PrivilegeImages");

            migrationBuilder.DropColumn(
                name: "IDC_Address",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IDC_Aumper",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IDC_Building",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IDC_Province",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IDC_Tumbon",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IDC_ZipCode",
                table: "Customers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffRoleID",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GoldLady",
                table: "Privileges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PerPersonLimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PerPrivilegeLimitType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Main",
                table: "PrivilegeImages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IDC_Address",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDC_Aumper",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDC_Building",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDC_Province",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IDC_Tumbon",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDC_ZipCode",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrivilegeType",
                columns: table => new
                {
                    PrivilegeTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PrivilegeTypeDesc = table.Column<string>(nullable: true),
                    PrivilegeTypeName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeType", x => x.PrivilegeTypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Privileges_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_PrivilegeType_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID",
                principalTable: "PrivilegeType",
                principalColumn: "PrivilegeTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
