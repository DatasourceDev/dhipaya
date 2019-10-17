using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class CleanAddressSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeMemberLevels_Privileges_PrivilegeID",
                table: "PrivilegeMemberLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_PrivilegeTypes_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropTable(
                name: "CustomerPrivileges");

            migrationBuilder.DropTable(
                name: "Geographys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeTypes",
                table: "PrivilegeTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeMemberLevels",
                table: "PrivilegeMemberLevels");

            migrationBuilder.DropColumn(
                name: "GeographyID",
                table: "Tumbons");

            migrationBuilder.DropColumn(
                name: "GeographyID",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "GeographyID",
                table: "Aumphurs");

            migrationBuilder.RenameTable(
                name: "PrivilegeTypes",
                newName: "PrivilegeType");

            migrationBuilder.RenameTable(
                name: "PrivilegeMemberLevels",
                newName: "PrivilegeMemberLevel");

            migrationBuilder.RenameIndex(
                name: "IX_PrivilegeMemberLevels_PrivilegeID",
                table: "PrivilegeMemberLevel",
                newName: "IX_PrivilegeMemberLevel_PrivilegeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeType",
                table: "PrivilegeType",
                column: "PrivilegeTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeMemberLevel",
                table: "PrivilegeMemberLevel",
                column: "PrivilegeMemberLevelID");

            migrationBuilder.CreateIndex(
                name: "IX_Tumbons_AumphurID",
                table: "Tumbons",
                column: "AumphurID");

            migrationBuilder.CreateIndex(
                name: "IX_Tumbons_ProvinceID",
                table: "Tumbons",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_Aumphurs_ProvinceID",
                table: "Aumphurs",
                column: "ProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Aumphurs_Provinces_ProvinceID",
                table: "Aumphurs",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeMemberLevel_Privileges_PrivilegeID",
                table: "PrivilegeMemberLevel",
                column: "PrivilegeID",
                principalTable: "Privileges",
                principalColumn: "PrivilegeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_PrivilegeType_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID",
                principalTable: "PrivilegeType",
                principalColumn: "PrivilegeTypeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tumbons_Aumphurs_AumphurID",
                table: "Tumbons",
                column: "AumphurID",
                principalTable: "Aumphurs",
                principalColumn: "AumphurID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tumbons_Provinces_ProvinceID",
                table: "Tumbons",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aumphurs_Provinces_ProvinceID",
                table: "Aumphurs");

            migrationBuilder.DropForeignKey(
                name: "FK_PrivilegeMemberLevel_Privileges_PrivilegeID",
                table: "PrivilegeMemberLevel");

            migrationBuilder.DropForeignKey(
                name: "FK_Privileges_PrivilegeType_PrivilegeTypeID",
                table: "Privileges");

            migrationBuilder.DropForeignKey(
                name: "FK_Tumbons_Aumphurs_AumphurID",
                table: "Tumbons");

            migrationBuilder.DropForeignKey(
                name: "FK_Tumbons_Provinces_ProvinceID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Tumbons_AumphurID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Tumbons_ProvinceID",
                table: "Tumbons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeType",
                table: "PrivilegeType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrivilegeMemberLevel",
                table: "PrivilegeMemberLevel");

            migrationBuilder.DropIndex(
                name: "IX_Aumphurs_ProvinceID",
                table: "Aumphurs");

            migrationBuilder.RenameTable(
                name: "PrivilegeType",
                newName: "PrivilegeTypes");

            migrationBuilder.RenameTable(
                name: "PrivilegeMemberLevel",
                newName: "PrivilegeMemberLevels");

            migrationBuilder.RenameIndex(
                name: "IX_PrivilegeMemberLevel_PrivilegeID",
                table: "PrivilegeMemberLevels",
                newName: "IX_PrivilegeMemberLevels_PrivilegeID");

            migrationBuilder.AddColumn<int>(
                name: "GeographyID",
                table: "Tumbons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GeographyID",
                table: "Provinces",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GeographyID",
                table: "Aumphurs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeTypes",
                table: "PrivilegeTypes",
                column: "PrivilegeTypeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrivilegeMemberLevels",
                table: "PrivilegeMemberLevels",
                column: "PrivilegeMemberLevelID");

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
                name: "Geographys",
                columns: table => new
                {
                    GeographyID = table.Column<int>(nullable: false),
                    GeographyName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Geographys", x => x.GeographyID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PrivilegeMemberLevels_Privileges_PrivilegeID",
                table: "PrivilegeMemberLevels",
                column: "PrivilegeID",
                principalTable: "Privileges",
                principalColumn: "PrivilegeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Privileges_PrivilegeTypes_PrivilegeTypeID",
                table: "Privileges",
                column: "PrivilegeTypeID",
                principalTable: "PrivilegeTypes",
                principalColumn: "PrivilegeTypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
