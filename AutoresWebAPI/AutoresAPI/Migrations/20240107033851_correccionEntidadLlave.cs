using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoresAPI.Migrations; 
/// <inheritdoc />
public partial class correccionEntidadLlave : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropForeignKey(
            name: "FK_LlavesAPI_AspNetUsers_IdentityUserId",
            table: "LlavesAPI");

        migrationBuilder.DropIndex(
            name: "IX_LlavesAPI_IdentityUserId",
            table: "LlavesAPI");

        migrationBuilder.DropColumn(
            name: "IdentityUserId",
            table: "LlavesAPI");

        migrationBuilder.AlterColumn<string>(
            name: "UsuarioId",
            table: "LlavesAPI",
            type: "nvarchar(450)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_LlavesAPI_UsuarioId",
            table: "LlavesAPI",
            column: "UsuarioId");

        migrationBuilder.AddForeignKey(
            name: "FK_LlavesAPI_AspNetUsers_UsuarioId",
            table: "LlavesAPI",
            column: "UsuarioId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropForeignKey(
            name: "FK_LlavesAPI_AspNetUsers_UsuarioId",
            table: "LlavesAPI");

        migrationBuilder.DropIndex(
            name: "IX_LlavesAPI_UsuarioId",
            table: "LlavesAPI");

        migrationBuilder.AlterColumn<string>(
            name: "UsuarioId",
            table: "LlavesAPI",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(450)",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "IdentityUserId",
            table: "LlavesAPI",
            type: "nvarchar(450)",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_LlavesAPI_IdentityUserId",
            table: "LlavesAPI",
            column: "IdentityUserId");

        migrationBuilder.AddForeignKey(
            name: "FK_LlavesAPI_AspNetUsers_IdentityUserId",
            table: "LlavesAPI",
            column: "IdentityUserId",
            principalTable: "AspNetUsers",
            principalColumn: "Id");
    }
}