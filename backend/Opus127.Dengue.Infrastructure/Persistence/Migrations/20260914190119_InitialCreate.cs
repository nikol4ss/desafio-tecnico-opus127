using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opus127.Dengue.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "DengueRecords",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Geocode = table.Column<int>(type: "int", nullable: false),
                EpidemiologicalYear = table.Column<int>(type: "int", nullable: false),
                EpidemiologicalWeek = table.Column<int>(type: "int", nullable: false),
                WeekStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                EstimatedCases = table.Column<double>(type: "float", nullable: false),
                EstimatedCasesMinimum = table.Column<double>(type: "float", nullable: true),
                EstimatedCasesMaximum = table.Column<double>(type: "float", nullable: true),
                NotifiedCases = table.Column<int>(type: "int", nullable: false),
                AlertLevel = table.Column<int>(type: "int", nullable: false),
                ProbabilityRtAboveOne = table.Column<double>(type: "float", nullable: true),
                EstimatedIncidencePer100K = table.Column<double>(type: "float", nullable: true),
                ReproductionNumber = table.Column<double>(type: "float", nullable: true),
                SourceRecordId = table.Column<long>(type: "bigint", nullable: true),
                ModelVersion = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                SyncedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_DengueRecords", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_DengueRecords_WeekStartDate",
            table: "DengueRecords",
            column: "WeekStartDate");

        migrationBuilder.CreateIndex(
            name: "UX_DengueRecords_Geocode_Year_Week",
            table: "DengueRecords",
            columns: new[] { "Geocode", "EpidemiologicalYear", "EpidemiologicalWeek" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "DengueRecords");
    }
}
