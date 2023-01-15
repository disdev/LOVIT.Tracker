using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOVIT.Tracker.Migrations
{
    /// <inheritdoc />
    public partial class CorrectLeaderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Checkins_CheckinId",
                table: "Leaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Checkpoints_CheckpointId",
                table: "Leaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Segments_SegmentId",
                table: "Leaders");

            migrationBuilder.RenameColumn(
                name: "SegmentId",
                table: "Leaders",
                newName: "LastSegmentId");

            migrationBuilder.RenameColumn(
                name: "CheckpointId",
                table: "Leaders",
                newName: "LastCheckpointId");

            migrationBuilder.RenameColumn(
                name: "CheckinId",
                table: "Leaders",
                newName: "LastCheckinId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_SegmentId",
                table: "Leaders",
                newName: "IX_Leaders_LastSegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_CheckpointId",
                table: "Leaders",
                newName: "IX_Leaders_LastCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_CheckinId",
                table: "Leaders",
                newName: "IX_Leaders_LastCheckinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Checkins_LastCheckinId",
                table: "Leaders",
                column: "LastCheckinId",
                principalTable: "Checkins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Checkpoints_LastCheckpointId",
                table: "Leaders",
                column: "LastCheckpointId",
                principalTable: "Checkpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Segments_LastSegmentId",
                table: "Leaders",
                column: "LastSegmentId",
                principalTable: "Segments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Checkins_LastCheckinId",
                table: "Leaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Checkpoints_LastCheckpointId",
                table: "Leaders");

            migrationBuilder.DropForeignKey(
                name: "FK_Leaders_Segments_LastSegmentId",
                table: "Leaders");

            migrationBuilder.RenameColumn(
                name: "LastSegmentId",
                table: "Leaders",
                newName: "SegmentId");

            migrationBuilder.RenameColumn(
                name: "LastCheckpointId",
                table: "Leaders",
                newName: "CheckpointId");

            migrationBuilder.RenameColumn(
                name: "LastCheckinId",
                table: "Leaders",
                newName: "CheckinId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_LastSegmentId",
                table: "Leaders",
                newName: "IX_Leaders_SegmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_LastCheckpointId",
                table: "Leaders",
                newName: "IX_Leaders_CheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaders_LastCheckinId",
                table: "Leaders",
                newName: "IX_Leaders_CheckinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Checkins_CheckinId",
                table: "Leaders",
                column: "CheckinId",
                principalTable: "Checkins",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Checkpoints_CheckpointId",
                table: "Leaders",
                column: "CheckpointId",
                principalTable: "Checkpoints",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Leaders_Segments_SegmentId",
                table: "Leaders",
                column: "SegmentId",
                principalTable: "Segments",
                principalColumn: "Id");
        }
    }
}
