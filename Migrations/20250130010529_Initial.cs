using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOVIT.Tracker.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: true),
                    End = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GeoJson = table.Column<string>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    From = table.Column<string>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: false),
                    FromCity = table.Column<string>(type: "TEXT", nullable: false),
                    FromState = table.Column<string>(type: "TEXT", nullable: false),
                    FromCountry = table.Column<string>(type: "TEXT", nullable: false),
                    FromZip = table.Column<string>(type: "TEXT", nullable: false),
                    Received = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Current = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Monitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CheckpointId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monitors_Checkpoints_CheckpointId",
                        column: x => x.CheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Races",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Distance = table.Column<float>(type: "REAL", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UltraSignupUrl = table.Column<string>(type: "TEXT", nullable: false),
                    RaceEventId = table.Column<Guid>(type: "TEXT", nullable: false),
                    GeoJson = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Races", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Races_RaceEvents_RaceEventId",
                        column: x => x.RaceEventId,
                        principalTable: "RaceEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Bib = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    Age = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<int>(type: "INTEGER", nullable: false),
                    RaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Linked = table.Column<bool>(type: "INTEGER", nullable: false),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Rank = table.Column<float>(type: "REAL", nullable: false),
                    UltraSignupEmail = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: true),
                    LinkCode = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    RaceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    GeoJson = table.Column<string>(type: "TEXT", nullable: false),
                    FromCheckpointId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ToCheckpointId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Distance = table.Column<double>(type: "REAL", nullable: false),
                    TotalDistance = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Segments_Checkpoints_FromCheckpointId",
                        column: x => x.FromCheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Segments_Checkpoints_ToCheckpointId",
                        column: x => x.ToCheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Segments_Races_RaceId",
                        column: x => x.RaceId,
                        principalTable: "Races",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Watchers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Disabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Watchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Watchers_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    When = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SegmentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Confirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: false),
                    MessageId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Elapsed = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkins_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Checkins_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Checkins_Segments_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "Segments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Leaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastCheckpointId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastSegmentId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastCheckinId = table.Column<Guid>(type: "TEXT", nullable: true),
                    NextPredictedSegmentTime = table.Column<uint>(type: "INTEGER", nullable: false),
                    OverallTime = table.Column<uint>(type: "INTEGER", nullable: false),
                    OverallPace = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leaders_Checkins_LastCheckinId",
                        column: x => x.LastCheckinId,
                        principalTable: "Checkins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Leaders_Checkpoints_LastCheckpointId",
                        column: x => x.LastCheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Leaders_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leaders_Segments_LastSegmentId",
                        column: x => x.LastSegmentId,
                        principalTable: "Segments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_MessageId",
                table: "Checkins",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_ParticipantId",
                table: "Checkins",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkins_SegmentId",
                table: "Checkins",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_LastCheckinId",
                table: "Leaders",
                column: "LastCheckinId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_LastCheckpointId",
                table: "Leaders",
                column: "LastCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_LastSegmentId",
                table: "Leaders",
                column: "LastSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_ParticipantId",
                table: "Leaders",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Monitors_CheckpointId",
                table: "Monitors",
                column: "CheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_RaceId",
                table: "Participants",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Races_RaceEventId",
                table: "Races",
                column: "RaceEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Segments_FromCheckpointId",
                table: "Segments",
                column: "FromCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Segments_RaceId",
                table: "Segments",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Segments_ToCheckpointId",
                table: "Segments",
                column: "ToCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Watchers_ParticipantId",
                table: "Watchers",
                column: "ParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertMessages");

            migrationBuilder.DropTable(
                name: "Leaders");

            migrationBuilder.DropTable(
                name: "Monitors");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Watchers");

            migrationBuilder.DropTable(
                name: "Checkins");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Segments");

            migrationBuilder.DropTable(
                name: "Checkpoints");

            migrationBuilder.DropTable(
                name: "Races");

            migrationBuilder.DropTable(
                name: "RaceEvents");
        }
    }
}
