using Microsoft.EntityFrameworkCore.Migrations;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class AddedOrderToCompositeKeyOfHopArrivals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HopArrivals",
                table: "HopArrivals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HopArrivals",
                table: "HopArrivals",
                columns: new[] { "ParcelTrackingId", "HopCode", "Order" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HopArrivals",
                table: "HopArrivals");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HopArrivals",
                table: "HopArrivals",
                columns: new[] { "ParcelTrackingId", "HopCode" });
        }
    }
}
