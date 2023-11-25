using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoresAPI.Migrations {
    /// <inheritdoc />
    public partial class DatosIniciales : Migration {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.InsertData(
                table: "Autores",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Lewis Carroll" },
                    { 2, "Fyodor Dostoevsky" },
                    { 3, "William Shakespeare" },
                    { 4, "Herman Melville" },
                    { 5, "Dante Alighieri" },
                    { 6, "Miguel De Cervantes" },
                    { 7, "Gabriel García Márquez" }
                });

            migrationBuilder.InsertData(
                table: "Libros",
                columns: new[] { "Id", "Titulo" },
                values: new object[,]
                {
                    { 1, "Alice’s Adventures In Wonderland" },
                    { 2, "Crime And Punishment" },
                    { 3, "Hamlet" },
                    { 4, "Moby Dick" },
                    { 5, "The Divine Comedy" },
                    { 6, "Don Quixote" },
                    { 7, "One Hundred Years Of Solitude" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Autores",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Libros",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}