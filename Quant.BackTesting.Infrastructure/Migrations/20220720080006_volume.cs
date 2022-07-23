using Microsoft.EntityFrameworkCore.Migrations;

namespace Quant.BackTesting.Infrastructure.Migrations
{
    public partial class volume : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Volume",
                table: "Stocks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Stocks");
        }
    }
}
