using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kofschip.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Verbs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Infinitive = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Stem = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    HasInseparablePrefix = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verbs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sentences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VerbId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tense = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Person = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Inversion = table.Column<bool>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sentences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sentences_Verbs_VerbId",
                        column: x => x.VerbId,
                        principalTable: "Verbs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_Tense",
                table: "Sentences",
                column: "Tense");

            migrationBuilder.CreateIndex(
                name: "IX_Sentences_VerbId",
                table: "Sentences",
                column: "VerbId");

            migrationBuilder.CreateIndex(
                name: "IX_Verbs_Infinitive",
                table: "Verbs",
                column: "Infinitive",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sentences");

            migrationBuilder.DropTable(
                name: "Verbs");
        }
    }
}
