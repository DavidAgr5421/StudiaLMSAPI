using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studia.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToSectionsAndActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "sections",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Visible");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "activities",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Visible");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "activities");
        }
    }
}
