using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomsBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissingForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomId",
                table: "bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_UserId",
                table: "bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_rooms_RoomId",
                table: "bookings",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_users_UserId",
                table: "bookings",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_rooms_RoomId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_bookings_users_UserId",
                table: "bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_bookings_RoomId",
                table: "bookings");

            migrationBuilder.DropIndex(
                name: "IX_bookings_UserId",
                table: "bookings");
        }
    }
}
