using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitRedeem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Redeems",
                columns: table => new
                {
                    RedeemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomerID = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Point = table.Column<decimal>(nullable: false),
                    PrivilegeID = table.Column<int>(nullable: false),
                    RedeemCode = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redeems", x => x.RedeemID);
                    table.ForeignKey(
                        name: "FK_Redeems_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Redeems_Privileges_PrivilegeID",
                        column: x => x.PrivilegeID,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Redeems_CustomerID",
                table: "Redeems",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Redeems_PrivilegeID",
                table: "Redeems",
                column: "PrivilegeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Redeems");
        }
    }
}
