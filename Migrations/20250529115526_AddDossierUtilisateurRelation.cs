using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISH_APP.Migrations
{
    /// <inheritdoc />
    public partial class AddDossierUtilisateurRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Contenu",
                table: "PieceJointe",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilisateurID",
                table: "Dossier",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "Client",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Client",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DossierUtilisateurs",
                columns: table => new
                {
                    DossierID = table.Column<int>(type: "int", nullable: false),
                    UtilisateurID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DossierUtilisateurs", x => new { x.DossierID, x.UtilisateurID });
                    table.ForeignKey(
                        name: "FK_DossierUtilisateurs_Dossier_DossierID",
                        column: x => x.DossierID,
                        principalTable: "Dossier",
                        principalColumn: "DossierID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DossierUtilisateurs_Utilisateur_UtilisateurID",
                        column: x => x.UtilisateurID,
                        principalTable: "Utilisateur",
                        principalColumn: "UtilisateurID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_UtilisateurID",
                table: "Dossier",
                column: "UtilisateurID");

            migrationBuilder.CreateIndex(
                name: "IX_DossierUtilisateurs_UtilisateurID",
                table: "DossierUtilisateurs",
                column: "UtilisateurID");

            migrationBuilder.AddForeignKey(
                name: "FK_Dossier_Utilisateur_UtilisateurID",
                table: "Dossier",
                column: "UtilisateurID",
                principalTable: "Utilisateur",
                principalColumn: "UtilisateurID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dossier_Utilisateur_UtilisateurID",
                table: "Dossier");

            migrationBuilder.DropTable(
                name: "DossierUtilisateurs");

            migrationBuilder.DropIndex(
                name: "IX_Dossier_UtilisateurID",
                table: "Dossier");

            migrationBuilder.DropColumn(
                name: "Contenu",
                table: "PieceJointe");

            migrationBuilder.DropColumn(
                name: "UtilisateurID",
                table: "Dossier");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "Client");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Client");
        }
    }
}
