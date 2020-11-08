using Microsoft.EntityFrameworkCore.Migrations;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class UpdatedHopParentWarehouseCodeCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "Hops");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "Hops",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}