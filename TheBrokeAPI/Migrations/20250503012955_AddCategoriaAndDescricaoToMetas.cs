using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBrokeAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaAndDescricaoToMetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "categoria",
                table: "metas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "descricao",
                table: "metas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "categoria",
                table: "metas");

            migrationBuilder.DropColumn(
                name: "descricao",
                table: "metas");
        }
    }
}
