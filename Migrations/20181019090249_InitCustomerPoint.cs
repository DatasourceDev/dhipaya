using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitCustomerPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPoints",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CustomerChanal = table.Column<int>(nullable: false),
                    CustomerID = table.Column<int>(nullable: false),
                    Point = table.Column<decimal>(nullable: false),
                    ProductID = table.Column<int>(nullable: true),
                    TransacionTypeID = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPoints", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CustomerPoints_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPoints_CustomerID",
                table: "CustomerPoints",
                column: "CustomerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPoints");
        }
    }
}
