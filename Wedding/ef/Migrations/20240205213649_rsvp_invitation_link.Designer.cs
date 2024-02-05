﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wedding.ef;

#nullable disable

namespace Wedding.ef.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240205213649_rsvp_invitation_link")]
    partial class rsvp_invitation_link
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Wedding.ef.Entities.Attendee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("InvitationId")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("InvitationId");

                    b.ToTable("Attendees");
                });

            modelBuilder.Entity("Wedding.ef.Entities.Invitation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<Guid>("PublicId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("SendTo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("SentTimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PublicId")
                        .IsUnique();

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("Wedding.ef.Entities.Rsvp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("InvitationId")
                        .HasColumnType("int");

                    b.Property<bool>("IsAttending")
                        .HasColumnType("bit");

                    b.Property<string>("Notes")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("InvitationId")
                        .IsUnique()
                        .HasFilter("[InvitationId] IS NOT NULL");

                    b.ToTable("Rsvps");
                });

            modelBuilder.Entity("Wedding.ef.Entities.Attendee", b =>
                {
                    b.HasOne("Wedding.ef.Entities.Invitation", null)
                        .WithMany("Attendees")
                        .HasForeignKey("InvitationId");
                });

            modelBuilder.Entity("Wedding.ef.Entities.Rsvp", b =>
                {
                    b.HasOne("Wedding.ef.Entities.Invitation", "Invitation")
                        .WithOne("Rsvp")
                        .HasForeignKey("Wedding.ef.Entities.Rsvp", "InvitationId");

                    b.Navigation("Invitation");
                });

            modelBuilder.Entity("Wedding.ef.Entities.Invitation", b =>
                {
                    b.Navigation("Attendees");

                    b.Navigation("Rsvp");
                });
#pragma warning restore 612, 618
        }
    }
}