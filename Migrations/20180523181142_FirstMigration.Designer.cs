﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PingPongPlanner.Models;
using System;

namespace PingPongPlanner.Migrations
{
    [DbContext(typeof(PingPongPlannerContext))]
    [Migration("20180523181142_FirstMigration")]
    partial class FirstMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("PingPongPlanner.Models.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("PostId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("PingPongPlanner.Models.Guest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MatchId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("PingPongPlanner.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<int>("CreatorId");

                    b.Property<DateTime>("Date");

                    b.Property<int>("LoserId");

                    b.Property<string>("PlayerOne");

                    b.Property<string>("PlayerTwo");

                    b.Property<int>("WinnerId");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("PingPongPlanner.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("MatchId");

                    b.Property<DateTime>("UpdatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("PostId");

                    b.HasIndex("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("PingPongPlanner.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created_At");

                    b.Property<string>("Email");

                    b.Property<int>("Losses");

                    b.Property<string>("Password");

                    b.Property<DateTime>("Updated_At");

                    b.Property<string>("Username");

                    b.Property<int>("Wins");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PingPongPlanner.Models.Comment", b =>
                {
                    b.HasOne("PingPongPlanner.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PingPongPlanner.Models.User", "Creator")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PingPongPlanner.Models.Guest", b =>
                {
                    b.HasOne("PingPongPlanner.Models.Match", "Match")
                        .WithMany("Guests")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PingPongPlanner.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PingPongPlanner.Models.Match", b =>
                {
                    b.HasOne("PingPongPlanner.Models.User", "Creator")
                        .WithMany("Matches")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("PingPongPlanner.Models.Post", b =>
                {
                    b.HasOne("PingPongPlanner.Models.Match", "Match")
                        .WithMany("Posts")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PingPongPlanner.Models.User", "Creator")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
