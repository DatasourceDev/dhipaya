using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AddPrivilege : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Customer_No",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Customer_Type",
                table: "Customers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerPrivileges",
                columns: table => new
                {
                    CustomerPrivilegeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Chanel = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CreditPoint = table.Column<decimal>(nullable: true),
                    CustomerID = table.Column<int>(nullable: true),
                    PrivilegeID = table.Column<int>(nullable: true),
                    SubmitDate = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPrivileges", x => x.CustomerPrivilegeID);
                });

            migrationBuilder.CreateTable(
                name: "Merchants",
                columns: table => new
                {
                    MerchantID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    MerchantName = table.Column<string>(nullable: true),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchants", x => x.MerchantID);
                });

            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    PrivilegeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    CreditPoint = table.Column<decimal>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    ImgUrl = table.Column<string>(nullable: true),
                    LogoUrl = table.Column<string>(nullable: true),
                    MerchantID = table.Column<int>(nullable: true),
                    PrivilegeCondition = table.Column<string>(nullable: true),
                    PrivilegeDesc = table.Column<string>(nullable: true),
                    PrivilegeName = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    Youtube = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeID);
                    table.ForeignKey(
                        name: "FK_Privileges_Merchants_MerchantID",
                        column: x => x.MerchantID,
                        principalTable: "Merchants",
                        principalColumn: "MerchantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Privileges_MerchantID",
                table: "Privileges",
                column: "MerchantID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPrivileges");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "Merchants");

            migrationBuilder.DropColumn(
                name: "Customer_No",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Customer_Type",
                table: "Customers");
        }
    }
}
