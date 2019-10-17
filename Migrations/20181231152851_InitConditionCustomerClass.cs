using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitConditionCustomerClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointConditionCustomerClasses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConditionID = table.Column<int>(nullable: false),
                    CustomerClassID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConditionCustomerClasses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointConditionCustomerClasses_PointConditions_ConditionID",
                        column: x => x.ConditionID,
                        principalTable: "PointConditions",
                        principalColumn: "ConditionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointConditionCustomerClasses_ConditionID",
                table: "PointConditionCustomerClasses",
                column: "ConditionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointConditionCustomerClasses");
        }
    }
}
