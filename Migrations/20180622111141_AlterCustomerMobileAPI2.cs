using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerMobileAPI2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VillageNo",
                table: "Customers",
                newName: "CUR_VillageNo");

            migrationBuilder.RenameColumn(
                name: "VillageNameEn",
                table: "Customers",
                newName: "CUR_VillageNameEn");

            migrationBuilder.RenameColumn(
                name: "VillageName",
                table: "Customers",
                newName: "CUR_VillageName");

            migrationBuilder.RenameColumn(
                name: "RoadEn",
                table: "Customers",
                newName: "CUR_RoadEn");

            migrationBuilder.RenameColumn(
                name: "Road",
                table: "Customers",
                newName: "CUR_Road");

            migrationBuilder.RenameColumn(
                name: "LaneEn",
                table: "Customers",
                newName: "CUR_LaneEn");

            migrationBuilder.RenameColumn(
                name: "Lane",
                table: "Customers",
                newName: "CUR_Lane");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CUR_VillageNo",
                table: "Customers",
                newName: "VillageNo");

            migrationBuilder.RenameColumn(
                name: "CUR_VillageNameEn",
                table: "Customers",
                newName: "VillageNameEn");

            migrationBuilder.RenameColumn(
                name: "CUR_VillageName",
                table: "Customers",
                newName: "VillageName");

            migrationBuilder.RenameColumn(
                name: "CUR_RoadEn",
                table: "Customers",
                newName: "RoadEn");

            migrationBuilder.RenameColumn(
                name: "CUR_Road",
                table: "Customers",
                newName: "Road");

            migrationBuilder.RenameColumn(
                name: "CUR_LaneEn",
                table: "Customers",
                newName: "LaneEn");

            migrationBuilder.RenameColumn(
                name: "CUR_Lane",
                table: "Customers",
                newName: "Lane");
        }
    }
}
