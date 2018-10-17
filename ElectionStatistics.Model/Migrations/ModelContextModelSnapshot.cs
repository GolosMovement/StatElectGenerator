﻿// <auto-generated />
using System;
using ElectionStatistics.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ElectionStatistics.Model.Migrations
{
    [DbContext(typeof(ModelContext))]
    partial class ModelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ElectionStatistics.Model.Candidate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("ElectionStatistics.Model.LineCalculatedValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("PresetId");

                    b.Property<int?>("ProtocolId");

                    b.Property<double>("Value");

                    b.HasKey("Id");

                    b.HasIndex("PresetId");

                    b.HasIndex("ProtocolId");

                    b.ToTable("LineCalculatedValues");
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineDescription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescriptionEng");

                    b.Property<string>("DescriptionNative");

                    b.Property<string>("DescriptionRus");

                    b.Property<bool>("IsCalcResult");

                    b.Property<bool>("IsVoteResult");

                    b.Property<int>("ProtocolSetId");

                    b.Property<string>("TitleEng");

                    b.Property<string>("TitleNative");

                    b.Property<string>("TitleRus")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ProtocolSetId");

                    b.ToTable("LineDescriptions");
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineNumber", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LineDescriptionId");

                    b.Property<int?>("ProtocolId");

                    b.Property<int>("Value");

                    b.HasKey("Id");

                    b.HasIndex("LineDescriptionId");

                    b.HasIndex("ProtocolId");

                    b.ToTable("LineNumbers");
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineString", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("LineDescriptionId");

                    b.Property<int?>("ProtocolId");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("LineDescriptionId");

                    b.HasIndex("ProtocolId");

                    b.ToTable("LineStrings");
                });

            modelBuilder.Entity("ElectionStatistics.Model.Mapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DataLineNumber");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Mappings");
                });

            modelBuilder.Entity("ElectionStatistics.Model.MappingLine", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ColumnNumber");

                    b.Property<string>("DescriptionEng");

                    b.Property<string>("DescriptionNative");

                    b.Property<string>("DescriptionRus");

                    b.Property<int>("HierarchyLanguage");

                    b.Property<int>("HierarchyLevel");

                    b.Property<bool>("IsCalcResult");

                    b.Property<bool>("IsHierarchy");

                    b.Property<bool>("IsNumber");

                    b.Property<bool>("IsVoteResult");

                    b.Property<int>("MappingId");

                    b.Property<string>("TitleEng");

                    b.Property<string>("TitleNative");

                    b.Property<string>("TitleRus");

                    b.HasKey("Id");

                    b.HasIndex("MappingId");

                    b.ToTable("MappingLines");
                });

            modelBuilder.Entity("ElectionStatistics.Model.Preset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescriptionEng");

                    b.Property<string>("DescriptionRus");

                    b.Property<string>("Expression")
                        .IsRequired();

                    b.Property<int>("ProtocolSetId");

                    b.Property<string>("TitleEng");

                    b.Property<string>("TitleRus")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ProtocolSetId");

                    b.ToTable("Presets");
                });

            modelBuilder.Entity("ElectionStatistics.Model.Protocol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommissionNumber");

                    b.Property<int?>("ParentId");

                    b.Property<int>("ProtocolSetId");

                    b.Property<string>("TitleEng");

                    b.Property<string>("TitleNative");

                    b.Property<string>("TitleRus")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("ProtocolSetId");

                    b.ToTable("Protocols");
                });

            modelBuilder.Entity("ElectionStatistics.Model.ProtocolSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DescriptionEng");

                    b.Property<string>("DescriptionRus")
                        .IsRequired();

                    b.Property<bool>("Hidden");

                    b.Property<int>("ImportCurrentLine");

                    b.Property<int>("ImportErrorCount");

                    b.Property<string>("ImportFileErrorLog");

                    b.Property<DateTime?>("ImportFinishedAt");

                    b.Property<DateTime?>("ImportStartedAt");

                    b.Property<bool>("ImportSuccess");

                    b.Property<int>("ImportTotalLines");

                    b.Property<bool>("ShouldRecalculatePresets");

                    b.Property<string>("TitleEng");

                    b.Property<string>("TitleRus")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("ProtocolSets");
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

            modelBuilder.Entity("ElectionStatistics.Model.LineCalculatedValue", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Preset", "Preset")
                        .WithMany()
                        .HasForeignKey("PresetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.Protocol", "Protocol")
                        .WithMany()
                        .HasForeignKey("ProtocolId");
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineDescription", b =>
                {
                    b.HasOne("ElectionStatistics.Model.ProtocolSet", "ProtocolSet")
                        .WithMany()
                        .HasForeignKey("ProtocolSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineNumber", b =>
                {
                    b.HasOne("ElectionStatistics.Model.LineDescription", "LineDescription")
                        .WithMany()
                        .HasForeignKey("LineDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.Protocol", "Protocol")
                        .WithMany("LineNumbers")
                        .HasForeignKey("ProtocolId");
                });

            modelBuilder.Entity("ElectionStatistics.Model.LineString", b =>
                {
                    b.HasOne("ElectionStatistics.Model.LineDescription", "LineDescription")
                        .WithMany()
                        .HasForeignKey("LineDescriptionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ElectionStatistics.Model.Protocol", "Protocol")
                        .WithMany("LineStrings")
                        .HasForeignKey("ProtocolId");
                });

            modelBuilder.Entity("ElectionStatistics.Model.MappingLine", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Mapping", "Mapping")
                        .WithMany()
                        .HasForeignKey("MappingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.Preset", b =>
                {
                    b.HasOne("ElectionStatistics.Model.ProtocolSet", "ProtocolSet")
                        .WithMany()
                        .HasForeignKey("ProtocolSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ElectionStatistics.Model.Protocol", b =>
                {
                    b.HasOne("ElectionStatistics.Model.Protocol", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.HasOne("ElectionStatistics.Model.ProtocolSet", "ProtocolSet")
                        .WithMany("Protocols")
                        .HasForeignKey("ProtocolSetId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
