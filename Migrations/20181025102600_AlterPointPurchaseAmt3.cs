using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Dhipaya.Migrations
{
   public partial class AlterPointPurchaseAmt3 : Migration
   {
      protected override void Up(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.AlterColumn<decimal>(
             name: "PurchaseAmt",
             table: "CustomerPoints",
             nullable: false,
             oldClrType: typeof(decimal),
             oldNullable: true);
      }

      protected override void Down(MigrationBuilder migrationBuilder)
      {
         migrationBuilder.AlterColumn<decimal>(
             name: "PurchaseAmt",
             table: "CustomerPoints",
             nullable: true,
             oldClrType: typeof(decimal));
      }
   }
}
