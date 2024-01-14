using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoresAPI.Migrations;

/// <inheritdoc />
public partial class Llaves : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "LlavesAPI",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Llave = table.Column<string>(type: "nvarchar(max)", nullable: false),
                TipoLlave = table.Column<int>(type: "int", nullable: false),
                Activa = table.Column<bool>(type: "bit", nullable: false),
                UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IdentityUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_LlavesAPI", x => x.Id);
                table.ForeignKey(
                    name: "FK_LlavesAPI_AspNetUsers_IdentityUserId",
                    column: x => x.IdentityUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_LlavesAPI_IdentityUserId",
            table: "LlavesAPI",
            column: "IdentityUserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "LlavesAPI");
    }
}