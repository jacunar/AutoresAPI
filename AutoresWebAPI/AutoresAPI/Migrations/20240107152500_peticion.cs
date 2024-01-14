using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoresAPI.Migrations;

/// <inheritdoc />
public partial class PeticionMigration : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "Peticiones",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LlaveAPIId = table.Column<int>(type: "int", nullable: false),
                FechaPeticion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Peticiones", x => x.Id);
                table.ForeignKey(
                    name: "FK_Peticiones_LlavesAPI_LlaveAPIId",
                    column: x => x.LlaveAPIId,
                    principalTable: "LlavesAPI",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Peticiones_LlaveAPIId",
            table: "Peticiones",
            column: "LlaveAPIId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "Peticiones");
    }
}