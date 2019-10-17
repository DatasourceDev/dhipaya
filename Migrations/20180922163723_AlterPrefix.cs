using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPrefix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PrefixTh",
                table: "Customers",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "PrefixEn",
                table: "Customers",
                nullable: true,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldDefaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PrefixTh",
                table: "Customers",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "PrefixEn",
                table: "Customers",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true,
                oldDefaultValue: 1);
        }
    }
}
