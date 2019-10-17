using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterUserRole4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_StaffRoles_StaffRoleID",
                table: "PageRoles");

            migrationBuilder.DropTable(
                name: "StaffRoles");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_StaffRoleID",
                table: "PageRoles");

            migrationBuilder.DropColumn(
                name: "StaffRoleID",
                table: "PageRoles");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserRoles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserRoles");

            migrationBuilder.AddColumn<int>(
                name: "StaffRoleID",
                table: "PageRoles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffRoles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    RoleName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffRoles", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageRoles_StaffRoleID",
                table: "PageRoles",
                column: "StaffRoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_PageRoles_StaffRoles_StaffRoleID",
                table: "PageRoles",
                column: "StaffRoleID",
                principalTable: "StaffRoles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
