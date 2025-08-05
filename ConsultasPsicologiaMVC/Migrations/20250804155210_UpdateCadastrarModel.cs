using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultasPsicologiaMVC.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCadastrarModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenhaDigitadaNovamente",
                table: "Cadastro");

            migrationBuilder.DropColumn(
                name: "Sobrenome",
                table: "Cadastro");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Cadastro",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Cadastro");

            migrationBuilder.AddColumn<string>(
                name: "SenhaDigitadaNovamente",
                table: "Cadastro",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sobrenome",
                table: "Cadastro",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
