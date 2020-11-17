using Microsoft.EntityFrameworkCore.Migrations;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class AddedOrderToHopArrivals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "HopArrivals",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "HopArrivals");
        }
    }
}
