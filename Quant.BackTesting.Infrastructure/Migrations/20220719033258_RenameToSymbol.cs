using Microsoft.EntityFrameworkCore.Migrations;

namespace Quant.BackTesting.Infrastructure.Migrations
{
    public partial class RenameToSymbol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StockDataId",
                table: "Stocks",
                newName: "SymbolDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SymbolDataId",
                table: "Stocks",
                newName: "StockDataId");
        }
    }
}
