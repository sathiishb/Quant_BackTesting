using Microsoft.EntityFrameworkCore.Migrations;

namespace Quant.BackTesting.Infrastructure.Migrations
{
    public partial class unique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Stocks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Symbol_DateTime",
                table: "Stocks",
                columns: new[] { "Symbol", "DateTime" },
                unique: true,
                filter: "[Symbol] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stocks_Symbol_DateTime",
                table: "Stocks");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
