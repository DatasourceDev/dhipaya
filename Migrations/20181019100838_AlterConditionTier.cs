using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterConditionTier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmountCalPoint",
                table: "PointConditions",
                newName: "CalPointPurchaseAmt");

            migrationBuilder.CreateTable(
                name: "PointConditionTier",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConditionID = table.Column<int>(nullable: false),
                    TierPercent = table.Column<decimal>(nullable: true),
                    TierPoint = table.Column<decimal>(nullable: true),
                    TierPurchaseAmtFrom = table.Column<decimal>(nullable: true),
                    TierPurchaseAmtTo = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConditionTier", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointConditionTier_PointConditions_ConditionID",
                        column: x => x.ConditionID,
                        principalTable: "PointConditions",
                        principalColumn: "ConditionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointConditionTier_ConditionID",
                table: "PointConditionTier",
                column: "ConditionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointConditionTier");

            migrationBuilder.RenameColumn(
                name: "CalPointPurchaseAmt",
                table: "PointConditions",
                newName: "AmountCalPoint");
        }
    }
}
