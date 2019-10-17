using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TelNo",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkTelNo",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerPrefixs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    NameEng = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPrefixs", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPrefixs");

            migrationBuilder.DropColumn(
                name: "TelNo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "WorkTelNo",
                table: "Customers");
        }
    }
}
