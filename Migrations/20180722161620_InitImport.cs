using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class InitImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "PrivilegeImpts",
                columns: table => new
                {
                    No = table.Column<int>(nullable: false),
                    Condition = table.Column<string>(nullable: true),
                    Gold = table.Column<string>(nullable: true),
                    Limit = table.Column<string>(nullable: true),
                    LimitPerPerson = table.Column<string>(nullable: true),
                    LimitPerPersonPeriod = table.Column<string>(nullable: true),
                    LimitPeriod = table.Column<string>(nullable: true),
                    MerchantName = table.Column<string>(nullable: true),
                    Outlets = table.Column<string>(nullable: true),
                    PeriodFrom = table.Column<string>(nullable: true),
                    PeriodTo = table.Column<string>(nullable: true),
                    PrivilegeName = table.Column<string>(nullable: true),
                    PrivilegeType = table.Column<string>(nullable: true),
                    ProvinceName = table.Column<string>(nullable: true),
                    Silver = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivilegeImpts", x => x.No);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivilegeImpts");
            
        }
    }
}
