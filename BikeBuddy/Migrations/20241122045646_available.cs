using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBuddy.Migrations
{
    /// <inheritdoc />
    public partial class available : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Bikes",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Bikes");
        }
    }
}
