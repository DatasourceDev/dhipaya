using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitShareholderImpt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShareholderImpts",
                columns: table => new
                {
                    AccountID = table.Column<string>(nullable: false),
                    Aumper = table.Column<string>(nullable: true),
                    Building = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    HTelNo = table.Column<string>(nullable: true),
                    IDCard = table.Column<string>(nullable: true),
                    Lane = table.Column<string>(nullable: true),
                    MoblieNo = table.Column<string>(nullable: true),
                    NameTh = table.Column<string>(nullable: true),
                    PrefixTh = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    Road = table.Column<string>(nullable: true),
                    SurNameTh = table.Column<string>(nullable: true),
                    Tumbon = table.Column<string>(nullable: true),
                    WTelNo = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareholderImpts", x => x.AccountID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareholderImpts");
        }
    }
}
