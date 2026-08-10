using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studia.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class AddCohortScopeAndActivityFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // defaultValueSql, no solo defaultValue: hay filas existentes (secciones/actividades
            // creadas antes de este cambio) y Postgres exige un DEFAULT real para agregar una
            // columna NOT NULL a una tabla no vacía.
            migrationBuilder.AddColumn<List<Guid>>(
                name: "CohortIds",
                table: "sections",
                type: "uuid[]",
                nullable: false,
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<List<Guid>>(
                name: "CohortIds",
                table: "activities",
                type: "uuid[]",
                nullable: false,
                defaultValueSql: "'{}'");

            migrationBuilder.AddColumn<string>(
                name: "_files",
                table: "activities",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CohortIds",
                table: "sections");

            migrationBuilder.DropColumn(
                name: "CohortIds",
                table: "activities");

            migrationBuilder.DropColumn(
                name: "_files",
                table: "activities");
        }
    }
}
