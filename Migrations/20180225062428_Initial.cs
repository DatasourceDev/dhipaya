using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false, defaultValue: 1),
                    UserName = table.Column<string>(nullable: false),
                    UserRoleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Users_UserRoles_UserRoleID",
                        column: x => x.UserRoleID,
                        principalTable: "UserRoles",
                        principalColumn: "UserRoleID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CUR_Address = table.Column<string>(nullable: true),
                    CUR_Aumper = table.Column<string>(nullable: true),
                    CUR_Building = table.Column<string>(nullable: true),
                    CUR_Province = table.Column<string>(nullable: true),
                    CUR_Tumbon = table.Column<string>(nullable: true),
                    CUR_ZipCode = table.Column<string>(nullable: true),
                    Create_By = table.Column<string>(nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: true),
                    Email = table.Column<string>(nullable: false),
                    IDC_Address = table.Column<string>(nullable: true),
                    IDC_Aumper = table.Column<string>(nullable: true),
                    IDC_Building = table.Column<string>(nullable: true),
                    IDC_Province = table.Column<string>(nullable: true),
                    IDC_Tumbon = table.Column<string>(nullable: true),
                    IDC_ZipCode = table.Column<string>(nullable: true),
                    IDCard = table.Column<string>(nullable: false),
                    IsDhiMember = table.Column<bool>(nullable: false),
                    LineID = table.Column<string>(nullable: true),
                    MoblieNo = table.Column<string>(nullable: false),
                    NameEn = table.Column<string>(nullable: false),
                    NameTh = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false, defaultValue: 1),
                    SurNameEn = table.Column<string>(nullable: false),
                    SurNameTh = table.Column<string>(nullable: false),
                    Update_By = table.Column<string>(nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    UserLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserID",
                table: "Customers",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRoleID",
                table: "Users",
                column: "UserRoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UserRoles");
        }
    }
}
