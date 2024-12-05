using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeBuddy.Migrations
{
    /// <inheritdoc />
    public partial class Contactno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNo",
                table: "Bikes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNo",
                table: "Bikes");
        }
    }
}
