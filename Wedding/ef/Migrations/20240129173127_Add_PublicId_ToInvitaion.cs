using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wedding.ef.Migrations
{
    /// <inheritdoc />
    public partial class Add_PublicId_ToInvitaion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "Invitations",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_PublicId",
                table: "Invitations",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invitations_PublicId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Invitations");
        }
    }
}
