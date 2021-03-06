// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TvShowScrubber.Contexts;

#nullable disable

namespace TvShowScrubber.Migrations
{
    [DbContext(typeof(ShowsDb))]
    [Migration("20220429105406_InitialCreate2")]
    partial class InitialCreate2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("TvShowScrubber.Models.Cast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PersonId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Casts");
                });

            modelBuilder.Entity("TvShowScrubber.Models.CastOverview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("EmbeddedCastId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PersonId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("EmbeddedCastId");

                    b.HasIndex("PersonId");

                    b.ToTable("CastOverview");
                });

            modelBuilder.Entity("TvShowScrubber.Models.EmbeddedCast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("EmbeddedCast");
                });

            modelBuilder.Entity("TvShowScrubber.Models.Show", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Shows");
                });

            modelBuilder.Entity("TvShowScrubber.Models.ShowWithCastEmbedded", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CastId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ShowId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CastId");

                    b.ToTable("ShowsWithCastEmbedded");
                });

            modelBuilder.Entity("TvShowScrubber.Models.CastOverview", b =>
                {
                    b.HasOne("TvShowScrubber.Models.EmbeddedCast", null)
                        .WithMany("CastOverviews")
                        .HasForeignKey("EmbeddedCastId");

                    b.HasOne("TvShowScrubber.Models.Cast", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Person");
                });

            modelBuilder.Entity("TvShowScrubber.Models.ShowWithCastEmbedded", b =>
                {
                    b.HasOne("TvShowScrubber.Models.EmbeddedCast", "Cast")
                        .WithMany()
                        .HasForeignKey("CastId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cast");
                });

            modelBuilder.Entity("TvShowScrubber.Models.EmbeddedCast", b =>
                {
                    b.Navigation("CastOverviews");
                });
#pragma warning restore 612, 618
        }
    }
}
