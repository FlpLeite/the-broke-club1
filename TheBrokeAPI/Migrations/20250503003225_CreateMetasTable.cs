using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TheBrokeAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateMetasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "metas",
                columns: table => new
                {
                    id_objetivo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    titulo = table.Column<string>(type: "text", nullable: false),
                    valor_meta = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_atual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_limite = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_metas", x => x.id_objetivo);
                    table.ForeignKey(
                        name: "FK_metas_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_metas_id_usuario",
                table: "metas",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "metas");
        }
    }
}
