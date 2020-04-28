using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerExport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerExports",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Address = table.Column<string>(nullable: true),
                    Channel = table.Column<string>(nullable: true),
                    Create_On = table.Column<string>(nullable: true),
                    CustomerClass = table.Column<string>(nullable: true),
                    DOB = table.Column<string>(nullable: true),
                    DOBMonth = table.Column<string>(nullable: true),
                    DOBYear = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    IDCard = table.Column<string>(nullable: true),
                    MoblieNo = table.Column<string>(nullable: true),
                    NameTh = table.Column<string>(nullable: true),
                    Point = table.Column<string>(nullable: true),
                    Province = table.Column<string>(nullable: true),
                    RefCode = table.Column<string>(nullable: true),
                    SurNameTh = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerExports", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerExports");
        }
    }
}
