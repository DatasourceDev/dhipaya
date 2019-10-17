using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterAdjustPoint1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConitionCode",
                table: "PointAdjusts",
                newName: "ConditionCode");

            migrationBuilder.AddColumn<int>(
                name: "CustomerChanal",
                table: "PointAdjusts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransacionTypeID",
                table: "PointAdjusts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerChanal",
                table: "PointAdjusts");

            migrationBuilder.DropColumn(
                name: "TransacionTypeID",
                table: "PointAdjusts");

            migrationBuilder.RenameColumn(
                name: "ConditionCode",
                table: "PointAdjusts",
                newName: "ConitionCode");
        }
    }
}
