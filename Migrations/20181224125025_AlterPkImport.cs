using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
    public partial class AlterPkImport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerImpts",
                table: "CustomerImpts");

            migrationBuilder.AlterColumn<string>(
                name: "No",
                table: "CustomerImpts",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "CustomerImpts",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerImpts",
                table: "CustomerImpts",
                column: "ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerImpts",
                table: "CustomerImpts");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "CustomerImpts");

            migrationBuilder.AlterColumn<int>(
                name: "No",
                table: "CustomerImpts",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerImpts",
                table: "CustomerImpts",
                column: "No");
        }
    }
}
