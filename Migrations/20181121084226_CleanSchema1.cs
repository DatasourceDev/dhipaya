using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class CleanSchema1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NewsActivityGroupID",
                table: "NewsActivities",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionGroupID",
                table: "Questions",
                column: "QuestionGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_PointConditionProducts_ProductID",
                table: "PointConditionProducts",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_PageRoles_PageID",
                table: "PageRoles",
                column: "PageID");

            migrationBuilder.CreateIndex(
                name: "IX_NewsActivities_NewsActivityGroupID",
                table: "NewsActivities",
                column: "NewsActivityGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsActivities_NewsActivityGroups_NewsActivityGroupID",
                table: "NewsActivities",
                column: "NewsActivityGroupID",
                principalTable: "NewsActivityGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageRoles_Pages_PageID",
                table: "PageRoles",
                column: "PageID",
                principalTable: "Pages",
                principalColumn: "PageID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PointConditionProducts_Products_ProductID",
                table: "PointConditionProducts",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_QuestionGroups_QuestionGroupID",
                table: "Questions",
                column: "QuestionGroupID",
                principalTable: "QuestionGroups",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsActivities_NewsActivityGroups_NewsActivityGroupID",
                table: "NewsActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_PageRoles_Pages_PageID",
                table: "PageRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_PointConditionProducts_Products_ProductID",
                table: "PointConditionProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_QuestionGroups_QuestionGroupID",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_QuestionGroupID",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_PointConditionProducts_ProductID",
                table: "PointConditionProducts");

            migrationBuilder.DropIndex(
                name: "IX_PageRoles_PageID",
                table: "PageRoles");

            migrationBuilder.DropIndex(
                name: "IX_NewsActivities_NewsActivityGroupID",
                table: "NewsActivities");

            migrationBuilder.DropColumn(
                name: "NewsActivityGroupID",
                table: "NewsActivities");
        }
    }
}
