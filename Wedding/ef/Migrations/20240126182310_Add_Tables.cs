using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.ef.Migrations
{
    /// <inheritdoc />
    public partial class Add_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Attendees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Attendees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "InvitationId",
                table: "Attendees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SendTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SentTimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rsvps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAttending = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InvitationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rsvps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rsvps_Invitations_InvitationId",
                        column: x => x.InvitationId,
                        principalTable: "Invitations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendees_InvitationId",
                table: "Attendees",
                column: "InvitationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rsvps_InvitationId",
                table: "Rsvps",
                column: "InvitationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendees_Invitations_InvitationId",
                table: "Attendees",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendees_Invitations_InvitationId",
                table: "Attendees");

            migrationBuilder.DropTable(
                name: "Rsvps");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Attendees_InvitationId",
                table: "Attendees");

            migrationBuilder.DropColumn(
                name: "InvitationId",
                table: "Attendees");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Attendees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
