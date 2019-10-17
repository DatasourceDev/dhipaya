using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class CreatePrivilegeType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrivilegeTypeID",
                table: "Privileges",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrivilegeTypes",
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
                    table.PrimaryKey("PK_PrivilegeTypes", x => x.PrivilegeTypeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Privileges_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_PrivilegeTypes_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID",
                principalTable: "PrivilegeTypes",
                principalColumn: "PrivilegeTypeID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_PrivilegeTypes_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropTable(
                name: "PrivilegeTypes");

            migrationBuilder.DropIndex(
                name: "IX_Privileges_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropColumn(
                name: "PrivilegeTypeID",
                table: "Privileges");
        }
    }
}
