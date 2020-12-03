using Microsoft.EntityFrameworkCore.Migrations;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class ChangedTypeOfWebhookIdToBigInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks"
            );

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Webhooks"
            );

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "Webhooks",
                nullable: false)
            .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks",
                column: "Id"
            );
            
            /*migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Webhooks",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks"
            );

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Webhooks"
            );

            migrationBuilder.AddColumn<int>(
                    name: "Id",
                    table: "Webhooks",
                    nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Webhooks",
                table: "Webhooks",
                column: "Id"
            );
            
            /*
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Webhooks",
                type: "int",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");*/
        }
    }
}
