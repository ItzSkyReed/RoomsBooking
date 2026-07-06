using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomsBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingExclusionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_bookings_RoomId_StartTime_EndTime",
                table: "bookings");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.Sql(
                @"ALTER TABLE bookings 
              ADD CONSTRAINT ""EXCLUDE_overlapping_bookings"" 
              EXCLUDE USING gist (
                  ""RoomId"" WITH =, 
                  tstzrange(""StartTime"", ""EndTime"", '[)') WITH &&
              );"
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:btree_gist", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateIndex(
                name: "IX_bookings_RoomId_StartTime_EndTime",
                table: "bookings",
                columns: new[] { "RoomId", "StartTime", "EndTime" },
                unique: true);

            migrationBuilder.Sql(@"ALTER TABLE bookings DROP CONSTRAINT ""EXCLUDE_overlapping_bookings"";");
        }
    }
}
