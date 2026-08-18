using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studia.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddColorAndCoverImageToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "courses",
                type: "character varying(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageFileName",
                table: "courses",
                type: "character varying(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CoverImageSizeBytes",
                table: "courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverImageStorageKey",
                table: "courses",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CoverImageFileName",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CoverImageSizeBytes",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "CoverImageStorageKey",
                table: "courses");
        }
    }
}
