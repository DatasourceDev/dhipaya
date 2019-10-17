using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitCustomerImpt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerImpts",
                columns: table => new
                {
                    No = table.Column<int>(nullable: false),
                    Aumper = table.Column<string>(nullable: true),
                    Building = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<string>(nullable: true),
                    CustomerNo = table.Column<string>(nullable: true),
                    CustomerType = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    FacebookFlag = table.Column<string>(nullable: true),
                    FriendCode = table.Column<string>(nullable: true),
                    IDCard = table.Column<string>(nullable: true),
                    IsDhiMember = table.Column<string>(nullable: true),
                    Lane = table.Column<string>(nullable: true),
                    LineID = table.Column<string>(nullable: true),
                    MoblieNo = table.Column<string>(nullable: true),
                    NameTh = table.Column<string>(nullable: true),
                    PrefixTh = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    RefCode = table.Column<string>(nullable: true),
                    Road = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    SurNameTh = table.Column<string>(nullable: true),
                    Tumbon = table.Column<string>(nullable: true),
                    UserLevel = table.Column<string>(nullable: true),
                    VillageName = table.Column<string>(nullable: true),
                    VillageNo = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerImpts", x => x.No);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerImpts");
        }
    }
}
