using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitCondition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointConditions",
                columns: table => new
                {
                    ConditionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AmountCalPoint = table.Column<decimal>(nullable: true),
                    CalPoint = table.Column<decimal>(nullable: true),
                    ConditionCode = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Gold = table.Column<bool>(nullable: false),
                    IsForBirthday = table.Column<bool>(nullable: false),
                    IsForWeekDay = table.Column<bool>(nullable: false),
                    IsFri = table.Column<bool>(nullable: false),
                    IsMon = table.Column<bool>(nullable: false),
                    IsSat = table.Column<bool>(nullable: false),
                    IsSun = table.Column<bool>(nullable: false),
                    IsThu = table.Column<bool>(nullable: false),
                    IsTue = table.Column<bool>(nullable: false),
                    IsWed = table.Column<bool>(nullable: false),
                    LimitedDay = table.Column<int>(nullable: true),
                    LimitedMonth = table.Column<int>(nullable: true),
                    LimitedOnce = table.Column<int>(nullable: true),
                    LimitedWeek = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Percent = table.Column<decimal>(nullable: true),
                    Period = table.Column<int>(nullable: false),
                    Point = table.Column<decimal>(nullable: true),
                    PointType = table.Column<int>(nullable: false),
                    Silver = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TransacionTypeID = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConditions", x => x.ConditionID);
                    table.ForeignKey(
                        name: "FK_PointConditions_PointTransacionTypes_TransacionTypeID",
                        column: x => x.TransacionTypeID,
                        principalTable: "PointTransacionTypes",
                        principalColumn: "TransacionTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PointConditionProducts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConditionID = table.Column<int>(nullable: false),
                    ProductID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointConditionProducts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointConditionProducts_PointConditions_ConditionID",
                        column: x => x.ConditionID,
                        principalTable: "PointConditions",
                        principalColumn: "ConditionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointConditionProducts_ConditionID",
                table: "PointConditionProducts",
                column: "ConditionID");

            migrationBuilder.CreateIndex(
                name: "IX_PointConditions_TransacionTypeID",
                table: "PointConditions",
                column: "TransacionTypeID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointConditionProducts");

            migrationBuilder.DropTable(
                name: "PointConditions");
        }
    }
}
