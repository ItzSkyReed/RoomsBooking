using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomsBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsedCitextForRoomNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "rooms",
                type: "citext",
                maxLength: 100,
                nullable: false,
                comment: "Номер кабинета",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldComment: "Номер кабинета");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "rooms",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                comment: "Номер кабинета",
                oldClrType: typeof(string),
                oldType: "citext",
                oldMaxLength: 100,
                oldComment: "Номер кабинета");
        }
    }
}
