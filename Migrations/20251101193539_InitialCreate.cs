using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RpgApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_DISPUTAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dt_Disputa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtacanteId = table.Column<int>(type: "int", nullable: false),
                    OponenteId = table.Column<int>(type: "int", nullable: false),
                    Tx_Narracao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DISPUTAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_HABILIDADE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dano = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_HABILIDADE", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_USUARIOS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    DataAcesso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Perfil = table.Column<string>(type: "nvarchar(max)", nullable: true, defaultValue: "Jogadô"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USUARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_PERSONAGENS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PontosVida = table.Column<int>(type: "int", nullable: false),
                    Forca = table.Column<int>(type: "int", nullable: false),
                    Defesa = table.Column<int>(type: "int", nullable: false),
                    Inteligencia = table.Column<int>(type: "int", nullable: false),
                    Classe = table.Column<int>(type: "int", nullable: false),
                    FotoPersonagem = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    Disputas = table.Column<int>(type: "int", nullable: false),
                    Vitorias = table.Column<int>(type: "int", nullable: false),
                    Derrotas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PERSONAGENS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_PERSONAGENS_TB_USUARIOS_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "TB_USUARIOS",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TB_ARMAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dano = table.Column<int>(type: "int", nullable: false),
                    PersonagemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ARMAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_ARMAS_TB_PERSONAGENS_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "TB_PERSONAGENS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PERSONAGENS_HABILIDADE",
                columns: table => new
                {
                    PersonagemId = table.Column<int>(type: "int", nullable: false),
                    HabilidadeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PERSONAGENS_HABILIDADE", x => new { x.PersonagemId, x.HabilidadeId });
                    table.ForeignKey(
                        name: "FK_TB_PERSONAGENS_HABILIDADE_TB_HABILIDADE_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "TB_HABILIDADE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_PERSONAGENS_HABILIDADE_TB_PERSONAGENS_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "TB_PERSONAGENS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TB_HABILIDADE",
                columns: new[] { "Id", "Dano", "Nome" },
                values: new object[,]
                {
                    { 1, 39, "Adormecer" },
                    { 2, 41, "Congelar" },
                    { 3, 37, "Hipnotizar" }
                });

            migrationBuilder.InsertData(
                table: "TB_USUARIOS",
                columns: new[] { "Id", "DataAcesso", "Email", "Foto", "Latitude", "Longitude", "PasswordHash", "PasswordSalt", "Perfil", "Username" },
                values: new object[] { 1, null, "seuEmail@gmail.com", null, -23.520024100000001, -46.596497999999997, new byte[] { 110, 138, 28, 164, 8, 221, 233, 153, 186, 59, 15, 72, 206, 251, 226, 226, 117, 133, 253, 75, 211, 0, 198, 28, 118, 187, 150, 95, 162, 49, 114, 181, 147, 70, 140, 198, 20, 119, 46, 221, 113, 30, 190, 168, 228, 132, 85, 191, 37, 184, 94, 200, 255, 55, 214, 241, 238, 60, 101, 206, 130, 195, 26, 122 }, new byte[] { 151, 221, 208, 119, 28, 93, 78, 36, 171, 160, 246, 73, 7, 6, 140, 202, 54, 17, 36, 230, 86, 10, 36, 46, 101, 64, 156, 208, 206, 33, 29, 182, 12, 255, 79, 219, 211, 31, 73, 156, 215, 3, 223, 232, 206, 196, 54, 218, 193, 227, 84, 4, 137, 232, 182, 101, 208, 4, 57, 187, 238, 3, 152, 78, 162, 197, 70, 95, 94, 122, 227, 104, 55, 74, 0, 159, 214, 207, 233, 202, 140, 182, 71, 20, 18, 150, 233, 20, 61, 230, 255, 115, 19, 229, 161, 117, 186, 247, 80, 113, 187, 164, 130, 186, 88, 69, 228, 9, 51, 154, 59, 26, 58, 35, 233, 117, 244, 74, 179, 229, 196, 187, 45, 151, 144, 85, 255, 81 }, "Admin", "UsuarioAdmin" });

            migrationBuilder.InsertData(
                table: "TB_PERSONAGENS",
                columns: new[] { "Id", "Classe", "Defesa", "Derrotas", "Disputas", "Forca", "FotoPersonagem", "Inteligencia", "Nome", "PontosVida", "UsuarioId", "Vitorias" },
                values: new object[,]
                {
                    { 1, 1, 23, 0, 0, 17, null, 33, "Frodo", 100, 1, 0 },
                    { 2, 1, 25, 0, 0, 15, null, 30, "Sam", 100, 1, 0 },
                    { 3, 3, 21, 0, 0, 18, null, 35, "Galadriel", 100, 1, 0 },
                    { 4, 2, 18, 0, 0, 18, null, 37, "Gandalf", 100, 1, 0 },
                    { 5, 1, 17, 0, 0, 20, null, 31, "Hobbit", 100, 1, 0 },
                    { 6, 3, 13, 0, 0, 21, null, 34, "Celeborn", 100, 1, 0 },
                    { 7, 2, 11, 0, 0, 25, null, 35, "Radagast", 100, 1, 0 }
                });

            migrationBuilder.InsertData(
                table: "TB_ARMAS",
                columns: new[] { "Id", "Dano", "Nome", "PersonagemId" },
                values: new object[,]
                {
                    { 1, 35, "Anti-Material Rifle", 1 },
                    { 2, 33, "Umbra", 2 },
                    { 3, 31, "Salame", 3 },
                    { 4, 30, "Blade of Chaos", 4 },
                    { 5, 34, "Marreita", 5 },
                    { 6, 33, "AK-43", 6 },
                    { 7, 32, "El machete", 7 }
                });

            migrationBuilder.InsertData(
                table: "TB_PERSONAGENS_HABILIDADE",
                columns: new[] { "HabilidadeId", "PersonagemId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 3, 3 },
                    { 3, 4 },
                    { 1, 5 },
                    { 2, 6 },
                    { 3, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ARMAS_PersonagemId",
                table: "TB_ARMAS",
                column: "PersonagemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_PERSONAGENS_UsuarioId",
                table: "TB_PERSONAGENS",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PERSONAGENS_HABILIDADE_HabilidadeId",
                table: "TB_PERSONAGENS_HABILIDADE",
                column: "HabilidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_ARMAS");

            migrationBuilder.DropTable(
                name: "TB_DISPUTAS");

            migrationBuilder.DropTable(
                name: "TB_PERSONAGENS_HABILIDADE");

            migrationBuilder.DropTable(
                name: "TB_HABILIDADE");

            migrationBuilder.DropTable(
                name: "TB_PERSONAGENS");

            migrationBuilder.DropTable(
                name: "TB_USUARIOS");
        }
    }
}
