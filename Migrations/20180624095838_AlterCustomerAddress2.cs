using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCustomerAddress2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aumphurs_Geographys_GeographyID",
                table: "Aumphurs");

            migrationBuilder.DropForeignKey(
                name: "FK_Aumphurs_Provinces_ProvinceID",
                table: "Aumphurs");

            migrationBuilder.DropForeignKey(
                name: "FK_Provinces_Geographys_GeographyID",
                table: "Provinces");

            migrationBuilder.DropForeignKey(
                name: "FK_Tumbons_Aumphurs_AumphurID",
                table: "Tumbons");

            migrationBuilder.DropForeignKey(
                name: "FK_Tumbons_Geographys_GeographyID",
                table: "Tumbons");

            migrationBuilder.DropForeignKey(
                name: "FK_Tumbons_Provinces_ProvinceID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Tumbons_AumphurID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Tumbons_GeographyID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Tumbons_ProvinceID",
                table: "Tumbons");

            migrationBuilder.DropIndex(
                name: "IX_Provinces_GeographyID",
                table: "Provinces");

            migrationBuilder.DropIndex(
                name: "IX_Aumphurs_GeographyID",
                table: "Aumphurs");

            migrationBuilder.DropIndex(
                name: "IX_Aumphurs_ProvinceID",
                table: "Aumphurs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_GeographyID",
                table: "Provinces",
                column: "GeographyID");

            migrationBuilder.CreateIndex(
                name: "IX_Aumphurs_GeographyID",
                table: "Aumphurs",
                column: "GeographyID");

            migrationBuilder.CreateIndex(
                name: "IX_Aumphurs_ProvinceID",
                table: "Aumphurs",
                column: "ProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Aumphurs_Geographys_GeographyID",
                table: "Aumphurs",
                column: "GeographyID",
                principalTable: "Geographys",
                principalColumn: "GeographyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Aumphurs_Provinces_ProvinceID",
                table: "Aumphurs",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Provinces_Geographys_GeographyID",
                table: "Provinces",
                column: "GeographyID",
                principalTable: "Geographys",
                principalColumn: "GeographyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tumbons_Aumphurs_AumphurID",
                table: "Tumbons",
                column: "AumphurID",
                principalTable: "Aumphurs",
                principalColumn: "AumphurID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tumbons_Geographys_GeographyID",
                table: "Tumbons",
                column: "GeographyID",
                principalTable: "Geographys",
                principalColumn: "GeographyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tumbons_Provinces_ProvinceID",
                table: "Tumbons",
                column: "ProvinceID",
                principalTable: "Provinces",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
