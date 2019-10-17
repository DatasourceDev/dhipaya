using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitPrivilegeCustomerClasses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerClassID",
                table: "Privileges");

            migrationBuilder.CreateTable(
                name: "PrivilegeCustomerClass",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerClassID = table.Column<int>(nullable: false),
                    PrivilegeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeCustomerClass", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PrivilegeCustomerClass_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivilegeCustomerClass_PrivilegeID",
                table: "PrivilegeCustomerClass",
                column: "PrivilegeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivilegeCustomerClass");

            migrationBuilder.AddColumn<int>(
                name: "CustomerClassID",
                table: "Privileges",
                nullable: true);
        }
    }
}
