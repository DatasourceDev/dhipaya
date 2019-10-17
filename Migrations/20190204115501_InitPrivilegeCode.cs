using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitPrivilegeCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrivilegeCodeType",
                table: "Privileges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PrivilegeCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    MaxUse = table.Column<int>(nullable: true),
                    PrivilegeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeCodes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PrivilegeCodes_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegeCodes_PrivilegeID",
                table: "PrivilegeCodes",
                column: "PrivilegeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivilegeCodes");

            migrationBuilder.DropColumn(
                name: "PrivilegeCodeType",
                table: "Privileges");
        }
    }
}
