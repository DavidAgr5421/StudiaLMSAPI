using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studia.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityKindAndGroupSubmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "submissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "submissions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowsLateSubmission",
                table: "activities",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "activities",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Individual");

            migrationBuilder.AddColumn<DateTime>(
                name: "ManuallyClosedAtUtc",
                table: "activities",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenDateUtc",
                table: "activities",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "submissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "submissions");

            migrationBuilder.DropColumn(
                name: "AllowsLateSubmission",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "ManuallyClosedAtUtc",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "OpenDateUtc",
                table: "activities");
        }
    }
}
