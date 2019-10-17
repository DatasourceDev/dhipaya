using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterCategoryImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "MerchantCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "MerchantCategories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "MerchantCategories");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "MerchantCategories");
        }
    }
}
