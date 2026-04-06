using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReservationAndCategoryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Editions_BookId",
                table: "Editions");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Reservations",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Editions_BookId_EditionNumber",
                table: "Editions",
                columns: new[] { "BookId", "EditionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_BookId_UserId",
                table: "Bookmarks",
                columns: new[] { "BookId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Editions_BookId_EditionNumber",
                table: "Editions");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_BookId_UserId",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Editions_BookId",
                table: "Editions",
                column: "BookId");
        }
    }
}
