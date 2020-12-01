using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class ChangedRegionGeometryTypeToNTSGeometry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Truck_RegionGeometry",
                table: "Hops"
            );
            
            migrationBuilder.AddColumn<Geometry>(
                name: "Truck_RegionGeometry",
                table: "Hops",
                type: "Geometry",
                nullable: true
            );
            
            migrationBuilder.DropColumn(
                name: "RegionGeometry",
                table: "Hops"
            );
            
            migrationBuilder.AddColumn<Geometry>(
                name: "RegionGeometry",
                table: "Hops",
                type: "Geometry",
                nullable: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Truck_RegionGeometry",
                table: "Hops"
            );
            
            migrationBuilder.AddColumn<Geometry>(
                name: "Truck_RegionGeometry",
                table: "Hops",
                nullable: true
            );
            
            migrationBuilder.DropColumn(
                name: "RegionGeometry",
                table: "Hops"
            );
            
            migrationBuilder.AddColumn<Geometry>(
                name: "RegionGeometry",
                table: "Hops",
                nullable: true
            );
        }
    }
}
