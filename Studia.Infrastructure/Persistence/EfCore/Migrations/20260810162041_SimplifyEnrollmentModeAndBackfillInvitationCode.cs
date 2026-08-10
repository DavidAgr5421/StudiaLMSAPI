using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Studia.Infrastructure.Persistence.EfCore.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyEnrollmentModeAndBackfillInvitationCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El modo "PorInvitacion" desaparece del enum: los cursos que lo tenían pasan a
            // ConAprobacion (el más parecido -- seguían sin permitir auto-servicio abierto).
            migrationBuilder.Sql(
                """
                UPDATE courses SET "EnrollmentMode" = 'ConAprobacion' WHERE "EnrollmentMode" = 'PorInvitacion';
                """);

            // El código de invitación ahora es obligatorio para todo curso, sin importar el
            // modo. Los cursos que no tenían uno (creados en modo Abierta/ConAprobacion antes
            // de este cambio) reciben uno generado acá.
            migrationBuilder.Sql(
                """
                UPDATE courses
                SET "InvitationCode" = upper(substr(md5(random()::text || "Id"::text), 1, 8))
                WHERE "InvitationCode" IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "InvitationCode",
                table: "courses",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InvitationCode",
                table: "courses",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);
        }
    }
}
