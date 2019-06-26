using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SZDC.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Railways",
                columns: table => new
                {
                    RailwayNumber = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Railways", x => x.RailwayNumber);
                });

            migrationBuilder.CreateTable(
                name: "StaticTrainEvent",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Hours = table.Column<int>(nullable: false),
                    Minutes = table.Column<int>(nullable: false),
                    HasMoreThan30Seconds = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticTrainEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Track",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Number = table.Column<string>(nullable: true),
                    TrackType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Track", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    TrainNumber = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrainType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.TrainNumber);
                });

            migrationBuilder.CreateTable(
                name: "StationStop",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StationName = table.Column<string>(nullable: true),
                    ArrivalId = table.Column<long>(nullable: true),
                    DepartureId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationStop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationStop_StaticTrainEvent_ArrivalId",
                        column: x => x.ArrivalId,
                        principalTable: "StaticTrainEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StationStop_StaticTrainEvent_DepartureId",
                        column: x => x.DepartureId,
                        principalTable: "StaticTrainEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StationStop_Station_StationName",
                        column: x => x.StationName,
                        principalTable: "Station",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderedTrack",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrackId = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    StationName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderedTrack", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderedTrack_Station_StationName",
                        column: x => x.StationName,
                        principalTable: "Station",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderedTrack_Track_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Track",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RailwaySection",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RailwayNumber = table.Column<string>(nullable: true),
                    TrainNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RailwaySection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RailwaySection_Railways_RailwayNumber",
                        column: x => x.RailwayNumber,
                        principalTable: "Railways",
                        principalColumn: "RailwayNumber",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RailwaySection_Trains_TrainNumber",
                        column: x => x.TrainNumber,
                        principalTable: "Trains",
                        principalColumn: "TrainNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StationStopOrder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StationStopId = table.Column<long>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    TrainNumber = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationStopOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationStopOrder_StationStop_StationStopId",
                        column: x => x.StationStopId,
                        principalTable: "StationStop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StationStopOrder_Trains_TrainNumber",
                        column: x => x.TrainNumber,
                        principalTable: "Trains",
                        principalColumn: "TrainNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RailwaySectionStation",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    StationName = table.Column<string>(nullable: false),
                    KilometersInSegment = table.Column<double>(nullable: false),
                    KilometersString = table.Column<string>(nullable: true),
                    StationOrder = table.Column<int>(nullable: false),
                    RailwaySectionId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RailwaySectionStation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RailwaySectionStation_RailwaySection_RailwaySectionId",
                        column: x => x.RailwaySectionId,
                        principalTable: "RailwaySection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RailwaySectionStation_Station_StationName",
                        column: x => x.StationName,
                        principalTable: "Station",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderedTrack_StationName",
                table: "OrderedTrack",
                column: "StationName");

            migrationBuilder.CreateIndex(
                name: "IX_OrderedTrack_TrackId",
                table: "OrderedTrack",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_RailwaySection_RailwayNumber",
                table: "RailwaySection",
                column: "RailwayNumber");

            migrationBuilder.CreateIndex(
                name: "IX_RailwaySection_TrainNumber",
                table: "RailwaySection",
                column: "TrainNumber");

            migrationBuilder.CreateIndex(
                name: "IX_RailwaySectionStation_RailwaySectionId",
                table: "RailwaySectionStation",
                column: "RailwaySectionId");

            migrationBuilder.CreateIndex(
                name: "IX_RailwaySectionStation_StationName",
                table: "RailwaySectionStation",
                column: "StationName");

            migrationBuilder.CreateIndex(
                name: "IX_StationStop_ArrivalId",
                table: "StationStop",
                column: "ArrivalId");

            migrationBuilder.CreateIndex(
                name: "IX_StationStop_DepartureId",
                table: "StationStop",
                column: "DepartureId");

            migrationBuilder.CreateIndex(
                name: "IX_StationStop_StationName",
                table: "StationStop",
                column: "StationName");

            migrationBuilder.CreateIndex(
                name: "IX_StationStopOrder_StationStopId",
                table: "StationStopOrder",
                column: "StationStopId");

            migrationBuilder.CreateIndex(
                name: "IX_StationStopOrder_TrainNumber",
                table: "StationStopOrder",
                column: "TrainNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderedTrack");

            migrationBuilder.DropTable(
                name: "RailwaySectionStation");

            migrationBuilder.DropTable(
                name: "StationStopOrder");

            migrationBuilder.DropTable(
                name: "Track");

            migrationBuilder.DropTable(
                name: "RailwaySection");

            migrationBuilder.DropTable(
                name: "StationStop");

            migrationBuilder.DropTable(
                name: "Railways");

            migrationBuilder.DropTable(
                name: "Trains");

            migrationBuilder.DropTable(
                name: "StaticTrainEvent");

            migrationBuilder.DropTable(
                name: "Station");
        }
    }
}
