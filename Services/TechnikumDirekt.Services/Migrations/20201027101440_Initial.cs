using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace technikumDirekt.Services.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hops",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    HopType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ProcessingDelayMins = table.Column<int>(nullable: true),
                    LocationName = table.Column<string>(nullable: true),
                    LocationCoordinates = table.Column<Point>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    ParentWarehouseCode = table.Column<string>(nullable: true),
                    ParentTraveltimeMins = table.Column<int>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    RegionGeometry = table.Column<Geometry>(nullable: true),
                    LogisticsPartner = table.Column<string>(nullable: true),
                    LogisticsPartnerUrl = table.Column<string>(nullable: true),
                    Truck_RegionGeometry = table.Column<Geometry>(nullable: true),
                    NumberPlate = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hops", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Hops_Hops_ParentWarehouseCode",
                        column: x => x.ParentWarehouseCode,
                        principalTable: "Hops",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parcels",
                columns: table => new
                {
                    TrackingId = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: true),
                    Weight = table.Column<float>(nullable: true),
                    RecipientId = table.Column<int>(nullable: true),
                    SenderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parcels", x => x.TrackingId);
                    table.ForeignKey(
                        name: "FK_Parcels_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parcels_Recipients_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Recipients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HopArrivals",
                columns: table => new
                {
                    ParcelTrackingId = table.Column<string>(nullable: false),
                    HopCode = table.Column<string>(nullable: false),
                    DateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HopArrivals", x => new { x.ParcelTrackingId, x.HopCode });
                    table.ForeignKey(
                        name: "FK_HopArrivals_Hops_HopCode",
                        column: x => x.HopCode,
                        principalTable: "Hops",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HopArrivals_Parcels_ParcelTrackingId",
                        column: x => x.ParcelTrackingId,
                        principalTable: "Parcels",
                        principalColumn: "TrackingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HopArrivals_HopCode",
                table: "HopArrivals",
                column: "HopCode");

            migrationBuilder.CreateIndex(
                name: "IX_Hops_ParentWarehouseCode",
                table: "Hops",
                column: "ParentWarehouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_RecipientId",
                table: "Parcels",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_Parcels_SenderId",
                table: "Parcels",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HopArrivals");

            migrationBuilder.DropTable(
                name: "Hops");

            migrationBuilder.DropTable(
                name: "Parcels");

            migrationBuilder.DropTable(
                name: "Recipients");
        }
    }
}
