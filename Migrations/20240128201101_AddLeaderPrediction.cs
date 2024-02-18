using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOVIT.Tracker.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaderPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "NextPredictedSegmentTime",
                table: "Leaders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextPredictedSegmentTime",
                table: "Leaders");
        }
    }
}
