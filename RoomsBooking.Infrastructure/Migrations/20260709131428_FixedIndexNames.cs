using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomsBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedIndexNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "ix_users_email");

            migrationBuilder.RenameIndex(
                name: "IX_rooms_Number",
                table: "rooms",
                newName: "ix_rooms_number");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                newName: "ix_refresh_tokens_token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "ix_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameIndex(
                name: "ix_rooms_number",
                table: "rooms",
                newName: "IX_rooms_Number");

            migrationBuilder.RenameIndex(
                name: "ix_refresh_tokens_token",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_Token");
        }
    }
}
