using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomsBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLengthLimitsToCitextFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"ALTER TABLE rooms ADD CONSTRAINT CK_rooms_number_max_length CHECK (char_length(""Number"") <= 100);
                ALTER TABLE users ADD CONSTRAINT CK_users_email_max_length CHECK (char_length(""Email"") <= 255);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
