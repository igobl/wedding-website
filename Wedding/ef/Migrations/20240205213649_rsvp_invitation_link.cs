using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.ef.Migrations
{
    /// <inheritdoc />
    public partial class rsvp_invitation_link : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rsvps_Invitations_InvitationId",
                table: "Rsvps");

            migrationBuilder.DropIndex(
                name: "IX_Rsvps_InvitationId",
                table: "Rsvps");

            migrationBuilder.AlterColumn<int>(
                name: "InvitationId",
                table: "Rsvps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Rsvps_InvitationId",
                table: "Rsvps",
                column: "InvitationId",
                unique: true,
                filter: "[InvitationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvps_Invitations_InvitationId",
                table: "Rsvps",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rsvps_Invitations_InvitationId",
                table: "Rsvps");

            migrationBuilder.DropIndex(
                name: "IX_Rsvps_InvitationId",
                table: "Rsvps");

            migrationBuilder.AlterColumn<int>(
                name: "InvitationId",
                table: "Rsvps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rsvps_InvitationId",
                table: "Rsvps",
                column: "InvitationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvps_Invitations_InvitationId",
                table: "Rsvps",
                column: "InvitationId",
                principalTable: "Invitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
