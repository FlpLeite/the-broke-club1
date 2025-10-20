using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TheBrokeAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateInvestimentosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "investimentos",
                columns: table => new
                {
                    id_investimento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    tipo = table.Column<string>(type: "text", nullable: false),
                    valor_investido = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_atual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_compra = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    corretora = table.Column<string>(type: "text", nullable: false),
                    notas = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_investimentos", x => x.id_investimento);
                    table.ForeignKey(
                        name: "FK_investimentos_usuario_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_investimentos_id_usuario",
                table: "investimentos",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "investimentos");
        }
    }
}