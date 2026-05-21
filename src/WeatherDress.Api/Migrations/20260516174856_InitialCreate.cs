using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherDress.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyRecommendations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ZipCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TemperatureC = table.Column<double>(type: "float", nullable: false),
                    WeatherDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeatherCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Jacket = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pants = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShoesNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyRecommendations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyRecommendations_ZipCode_Date",
                table: "DailyRecommendations",
                columns: new[] { "ZipCode", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyRecommendations");
        }
    }
}
