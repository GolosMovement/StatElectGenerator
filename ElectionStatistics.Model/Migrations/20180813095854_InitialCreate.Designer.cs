﻿// <auto-generated />
using ElectionStatistics.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ElectionStatistics.Model.Migrations
{
    [DbContext(typeof(ModelContext))]
    [Migration("20180813095854_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ElectionStatistics.Model.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GenitiveName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("ElectionStatistics.Model.Election", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DataSourceUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<DateTime>("Date");

                    b.Property<int>("ElectoralDistrictId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("ElectoralDistrictId");

                    b.ToTable("Elections");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionCandidate", b =>
                {
                    b.Property<int>("ElectionId");

                    b.Property<int>("CandidateId");

                    b.HasKey("ElectionId", "CandidateId");

                    b.HasIndex("CandidateId");

                    b.ToTable("ElectionCandidates");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionCandidateVote", b =>
                {
                    b.Property<int>("ElectionResultId");

                    b.Property<int>("CandidateId");

                    b.Property<int>("Count");

                    b.HasKey("ElectionResultId", "CandidateId");

                    b.HasIndex("CandidateId");

                    b.ToTable("ElectionCandidateVotes");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AbsenteeCertificateVotersCount");

                    b.Property<int>("CanceledAbsenteeCertificatesCount");

                    b.Property<int>("CanceledBallotsCount");

                    b.Property<string>("DataSourceUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.Property<int>("EarlyIssuedBallotsCount");

                    b.Property<int>("ElectionId");

                    b.Property<int>("ElectoralDistrictId");

                    b.Property<int>("InsideBallotsCount");

                    b.Property<int>("InvalidBallotsCount");

                    b.Property<int>("IssuedAbsenteeCertificatesCount");

                    b.Property<int>("IssuedByHigherDistrictAbsenteeCertificatesCount");

                    b.Property<int>("IssuedInsideBallotsCount");

                    b.Property<int>("IssuedOutsideBallotsCount");

                    b.Property<int>("LostAbsenteeCertificatesCount");

                    b.Property<int>("LostBallotsCount");

                    b.Property<int>("OutsideBallotsCount");

                    b.Property<int>("ReceivedAbsenteeCertificatesCount");

                    b.Property<int>("ReceivedBallotsCount");

                    b.Property<int>("UnaccountedBallotsCount");

                    b.Property<int>("ValidBallotsCount");

                    b.Property<int>("VotersCount");

                    b.HasKey("Id");

                    b.HasIndex("ElectionId");

                    b.HasIndex("ElectoralDistrictId");

                    b.ToTable("ElectionResults");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectoralDistrict", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HierarchyPath")
                        .HasMaxLength(100);

                    b.Property<int?>("HigherDistrictId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("HigherDistrictId");

                    b.ToTable("ElectoralDistricts");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectoralDistrictElection", b =>
                {
                    b.Property<int>("ElectionId");

                    b.Property<int>("ElectoralDistrictId");

                    b.Property<string>("DataSourceUrl")
                        .IsRequired()
                        .HasMaxLength(1000);

                    b.HasKey("ElectionId", "ElectoralDistrictId");

                    b.HasIndex("ElectoralDistrictId");

                    b.ToTable("ElectoralDistrictElections");
                });

            modelBuilder.Entity("ElectionStatistics.Model.Election", b =>
                {
                    b.HasOne("ElectionStatistics.Model.ElectoralDistrict", "ElectoralDistrict")
                        .WithMany()
                        .HasForeignKey("ElectoralDistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionCandidate", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Candidate", "Candidate")
                        .WithMany("Elections")
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.Election", "Election")
                        .WithMany("Candidates")
                        .HasForeignKey("ElectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionCandidateVote", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Candidate", "Candidate")
                        .WithMany()
                        .HasForeignKey("CandidateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.ElectionResult", "ElectionResult")
                        .WithMany("Votes")
                        .HasForeignKey("ElectionResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectionResult", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Election", "Election")
                        .WithMany()
                        .HasForeignKey("ElectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.ElectoralDistrict", "ElectoralDistrict")
                        .WithMany()
                        .HasForeignKey("ElectoralDistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectoralDistrict", b =>
                {
                    b.HasOne("ElectionStatistics.Model.ElectoralDistrict", "HigherDistrict")
                        .WithMany("LowerDistricts")
                        .HasForeignKey("HigherDistrictId");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ElectoralDistrictElection", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Election", "Election")
                        .WithMany()
                        .HasForeignKey("ElectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.ElectoralDistrict", "ElectoralDistrict")
                        .WithMany()
                        .HasForeignKey("ElectoralDistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
