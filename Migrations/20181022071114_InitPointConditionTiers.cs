using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitPointConditionTiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointConditionTier_PointConditions_ConditionID",
                table: "PointConditionTier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointConditionTier",
                table: "PointConditionTier");

            migrationBuilder.RenameTable(
                name: "PointConditionTier",
                newName: "PointConditionTiers");

            migrationBuilder.RenameColumn(
                name: "TierPurchaseAmtTo",
                table: "PointConditionTiers",
                newName: "PurchaseAmtTo");

            migrationBuilder.RenameColumn(
                name: "TierPurchaseAmtFrom",
                table: "PointConditionTiers",
                newName: "PurchaseAmtFrom");

            migrationBuilder.RenameColumn(
                name: "TierPoint",
                table: "PointConditionTiers",
                newName: "Point");

            migrationBuilder.RenameColumn(
                name: "TierPercent",
                table: "PointConditionTiers",
                newName: "Percent");

            migrationBuilder.RenameIndex(
                name: "IX_PointConditionTier_ConditionID",
                table: "PointConditionTiers",
                newName: "IX_PointConditionTiers_ConditionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointConditionTiers",
                table: "PointConditionTiers",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditionTiers_PointConditions_ConditionID",
                table: "PointConditionTiers",
                column: "ConditionID",
                principalTable: "PointConditions",
                principalColumn: "ConditionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointConditionTiers_PointConditions_ConditionID",
                table: "PointConditionTiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointConditionTiers",
                table: "PointConditionTiers");

            migrationBuilder.RenameTable(
                name: "PointConditionTiers",
                newName: "PointConditionTier");

            migrationBuilder.RenameColumn(
                name: "PurchaseAmtTo",
                table: "PointConditionTier",
                newName: "TierPurchaseAmtTo");

            migrationBuilder.RenameColumn(
                name: "PurchaseAmtFrom",
                table: "PointConditionTier",
                newName: "TierPurchaseAmtFrom");

            migrationBuilder.RenameColumn(
                name: "Point",
                table: "PointConditionTier",
                newName: "TierPoint");

            migrationBuilder.RenameColumn(
                name: "Percent",
                table: "PointConditionTier",
                newName: "TierPercent");

            migrationBuilder.RenameIndex(
                name: "IX_PointConditionTiers_ConditionID",
                table: "PointConditionTier",
                newName: "IX_PointConditionTier_ConditionID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointConditionTier",
                table: "PointConditionTier",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditionTier_PointConditions_ConditionID",
                table: "PointConditionTier",
                column: "ConditionID",
                principalTable: "PointConditions",
                principalColumn: "ConditionID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
