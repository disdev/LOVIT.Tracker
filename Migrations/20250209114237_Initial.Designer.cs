﻿// <auto-generated />
using System;
using LOVIT.Tracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LOVIT.Tracker.Migrations
{
    [DbContext(typeof(TrackerContext))]
    [Migration("20250209114237_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LOVIT.Tracker.Models.AlertMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("End")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AlertMessages");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Checkin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.Property<long>("Elapsed")
                        .HasColumnType("bigint");

                    b.Property<Guid>("MessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("When")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("MessageId");

                    b.HasIndex("ParticipantId");

                    b.HasIndex("SegmentId");

                    b.ToTable("Checkins");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Checkpoint", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GeoJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Number")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Checkpoints");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Leader", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LastCheckinId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LastCheckpointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("LastSegmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("NextPredictedSegmentTime")
                        .HasColumnType("bigint");

                    b.Property<long>("OverallPace")
                        .HasColumnType("bigint");

                    b.Property<long>("OverallTime")
                        .HasColumnType("bigint");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("LastCheckinId");

                    b.HasIndex("LastCheckpointId");

                    b.HasIndex("LastSegmentId");

                    b.HasIndex("ParticipantId");

                    b.ToTable("Leaders");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromCity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromCountry")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromState")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromZip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Received")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Monitor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("CheckpointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CheckpointId");

                    b.ToTable("Monitors");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Age")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Bib")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("LinkCode")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Linked")
                        .HasColumnType("bit");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("Rank")
                        .HasColumnType("real");

                    b.Property<string>("Region")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UltraSignupEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RaceId");

                    b.ToTable("Participants");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Race", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Distance")
                        .HasColumnType("real");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<string>("GeoJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RaceEventId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("UltraSignupUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RaceEventId");

                    b.ToTable("Races");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.RaceEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Current")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("RaceEvents");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Segment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Distance")
                        .HasColumnType("float");

                    b.Property<Guid?>("FromCheckpointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("GeoJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<Guid>("RaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ToCheckpointId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("TotalDistance")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("FromCheckpointId");

                    b.HasIndex("RaceId");

                    b.HasIndex("ToCheckpointId");

                    b.ToTable("Segments");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Setting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Watcher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Disabled")
                        .HasColumnType("bit");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("Watchers");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Checkin", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Message", "Message")
                        .WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LOVIT.Tracker.Models.Participant", "Participant")
                        .WithMany("Checkins")
                        .HasForeignKey("ParticipantId");

                    b.HasOne("LOVIT.Tracker.Models.Segment", "Segment")
                        .WithMany()
                        .HasForeignKey("SegmentId");

                    b.Navigation("Message");

                    b.Navigation("Participant");

                    b.Navigation("Segment");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Leader", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Checkin", "LastCheckin")
                        .WithMany()
                        .HasForeignKey("LastCheckinId");

                    b.HasOne("LOVIT.Tracker.Models.Checkpoint", "LastCheckpoint")
                        .WithMany()
                        .HasForeignKey("LastCheckpointId");

                    b.HasOne("LOVIT.Tracker.Models.Segment", "LastSegment")
                        .WithMany()
                        .HasForeignKey("LastSegmentId");

                    b.HasOne("LOVIT.Tracker.Models.Participant", "Participant")
                        .WithMany()
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LastCheckin");

                    b.Navigation("LastCheckpoint");

                    b.Navigation("LastSegment");

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Monitor", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Checkpoint", "Checkpoint")
                        .WithMany("Monitors")
                        .HasForeignKey("CheckpointId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Checkpoint");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Participant", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Race", "Race")
                        .WithMany("Participants")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Race");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Race", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.RaceEvent", "RaceEvent")
                        .WithMany("Races")
                        .HasForeignKey("RaceEventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RaceEvent");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Segment", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Checkpoint", "FromCheckpoint")
                        .WithMany()
                        .HasForeignKey("FromCheckpointId");

                    b.HasOne("LOVIT.Tracker.Models.Race", "Race")
                        .WithMany("Segments")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LOVIT.Tracker.Models.Checkpoint", "ToCheckpoint")
                        .WithMany()
                        .HasForeignKey("ToCheckpointId");

                    b.Navigation("FromCheckpoint");

                    b.Navigation("Race");

                    b.Navigation("ToCheckpoint");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Watcher", b =>
                {
                    b.HasOne("LOVIT.Tracker.Models.Participant", "Participant")
                        .WithMany("Watchers")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Checkpoint", b =>
                {
                    b.Navigation("Monitors");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Participant", b =>
                {
                    b.Navigation("Checkins");

                    b.Navigation("Watchers");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.Race", b =>
                {
                    b.Navigation("Participants");

                    b.Navigation("Segments");
                });

            modelBuilder.Entity("LOVIT.Tracker.Models.RaceEvent", b =>
                {
                    b.Navigation("Races");
                });
#pragma warning restore 612, 618
        }
    }
}
