using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOVIT.Tracker.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantLinkCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LinkCode",
                table: "Participants",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkCode",
                table: "Participants");
        }
    }
}
