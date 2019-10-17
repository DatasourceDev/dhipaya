using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    ProvinceID = table.Column<int>(nullable: false),
                    GeographyID = table.Column<int>(nullable: false),
                    ProvinceCode = table.Column<string>(nullable: true),
                    ProvinceName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.ProvinceID);
                    table.ForeignKey(
                        name: "FK_Provinces_Geographys_GeographyID",
                        column: x => x.GeographyID,
                        principalTable: "Geographys",
                        principalColumn: "GeographyID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aumphurs",
                columns: table => new
                {
                    AumphurID = table.Column<int>(nullable: false),
                    AumphurCode = table.Column<string>(nullable: true),
                    AumphurName = table.Column<string>(nullable: true),
                    GeographyID = table.Column<int>(nullable: false),
                    ProvinceID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aumphurs", x => x.AumphurID);
                    table.ForeignKey(
                        name: "FK_Aumphurs_Geographys_GeographyID",
                        column: x => x.GeographyID,
                        principalTable: "Geographys",
                        principalColumn: "GeographyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Aumphurs_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tumbons",
                columns: table => new
                {
                    TumbonID = table.Column<int>(nullable: false),
                    AumphurID = table.Column<int>(nullable: false),
                    GeographyID = table.Column<int>(nullable: false),
                    ProvinceID = table.Column<int>(nullable: false),
                    TumbonCode = table.Column<string>(nullable: true),
                    TumbonName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tumbons", x => x.TumbonID);
                    table.ForeignKey(
                        name: "FK_Tumbons_Aumphurs_AumphurID",
                        column: x => x.AumphurID,
                        principalTable: "Aumphurs",
                        principalColumn: "AumphurID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tumbons_Geographys_GeographyID",
                        column: x => x.GeographyID,
                        principalTable: "Geographys",
                        principalColumn: "GeographyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tumbons_Provinces_ProvinceID",
                        column: x => x.ProvinceID,
                        principalTable: "Provinces",
                        principalColumn: "ProvinceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aumphurs_GeographyID",
                table: "Aumphurs",
                column: "GeographyID");

            migrationBuilder.CreateIndex(
                name: "IX_Aumphurs_ProvinceID",
                table: "Aumphurs",
                column: "ProvinceID");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_GeographyID",
                table: "Provinces",
                column: "GeographyID");

            migrationBuilder.CreateIndex(
                name: "IX_Tumbons_AumphurID",
                table: "Tumbons",
                column: "AumphurID");

            migrationBuilder.CreateIndex(
                name: "IX_Tumbons_GeographyID",
                table: "Tumbons",
                column: "GeographyID");

            migrationBuilder.CreateIndex(
                name: "IX_Tumbons_ProvinceID",
                table: "Tumbons",
                column: "ProvinceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tumbons");

            migrationBuilder.DropTable(
                name: "Aumphurs");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "Geographys");
        }
    }
}
