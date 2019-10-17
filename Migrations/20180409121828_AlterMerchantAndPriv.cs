using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterMerchantAndPriv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Allowable_Outlet",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsage",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsagePerPerson",
                table: "Privileges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Merchants",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MerchantCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryName = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantCategories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "PrivilegeMemberLevels",
                columns: table => new
                {
                    PrivilegeMemberLevelID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberLevel = table.Column<string>(nullable: true),
                    Percent = table.Column<decimal>(nullable: true),
                    PrivilegeID = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeMemberLevels", x => x.PrivilegeMemberLevelID);
                    table.ForeignKey(
                        name: "FK_PrivilegeMemberLevels_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Merchants_CategoryID",
                table: "Merchants",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegeMemberLevels_PrivilegeID",
                table: "PrivilegeMemberLevels",
                column: "PrivilegeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Merchants_MerchantCategories_CategoryID",
                table: "Merchants",
                column: "CategoryID",
                principalTable: "MerchantCategories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Merchants_MerchantCategories_CategoryID",
                table: "Merchants");

            migrationBuilder.DropTable(
                name: "MerchantCategories");

            migrationBuilder.DropTable(
                name: "PrivilegeMemberLevels");

            migrationBuilder.DropIndex(
                name: "IX_Merchants_CategoryID",
                table: "Merchants");

            migrationBuilder.DropColumn(
                name: "Allowable_Outlet",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "MaxUsage",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "MaxUsagePerPerson",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Merchants");
        }
    }
}
