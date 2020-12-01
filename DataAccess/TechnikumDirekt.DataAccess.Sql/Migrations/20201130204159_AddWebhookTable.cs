using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TechnikumDirekt.DataAccess.Sql.Migrations
{
    public partial class AddWebhookTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Webhooks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParcelTrackingId = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Webhooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Webhooks_Parcels_ParcelTrackingId",
                        column: x => x.ParcelTrackingId,
                        principalTable: "Parcels",
                        principalColumn: "TrackingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Webhooks_ParcelTrackingId",
                table: "Webhooks",
                column: "ParcelTrackingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Webhooks");
        }
    }
}
