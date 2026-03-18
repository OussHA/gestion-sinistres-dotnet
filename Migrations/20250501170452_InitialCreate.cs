using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISH_APP.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assurance",
                columns: table => new
                {
                    AssuranceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Services = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodePostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SiteWeb = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Assuranc__CB6737D6B5B8C069", x => x.AssuranceID);
                });

            migrationBuilder.CreateTable(
                name: "Prestataire",
                columns: table => new
                {
                    PrestataireID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Services = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TarifNormal = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TarifSoirWeekendFerie = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Agences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AdresseSiege = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodePostalSiege = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VilleSiege = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Horaires = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SiteWeb = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Note = table.Column<decimal>(type: "decimal(3,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prestata__0CB38DF24A9706F3", x => x.PrestataireID);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateur",
                columns: table => new
                {
                    UtilisateurID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Utilisat__6CB6AE1FE8FEDB5B", x => x.UtilisateurID);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateNaissance = table.Column<DateOnly>(type: "date", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodePostal = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssuranceID = table.Column<int>(type: "int", nullable: true),
                    TypeAssurance = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Client__E67E1A04EAA5A29F", x => x.ClientID);
                    table.ForeignKey(
                        name: "FK__Client__Assuranc__3E52440B",
                        column: x => x.AssuranceID,
                        principalTable: "Assurance",
                        principalColumn: "AssuranceID");
                });

            migrationBuilder.CreateTable(
                name: "Dossier",
                columns: table => new
                {
                    DossierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientID = table.Column<int>(type: "int", nullable: true),
                    AssuranceID = table.Column<int>(type: "int", nullable: true),
                    PrestataireID = table.Column<int>(type: "int", nullable: true),
                    DateDeclaration = table.Column<DateOnly>(type: "date", nullable: true),
                    TypeSinistre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Priorite = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Etat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateDebutTravaux = table.Column<DateOnly>(type: "date", nullable: true),
                    DateFinTravaux = table.Column<DateOnly>(type: "date", nullable: true),
                    CommentaireISH = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentaireAssurance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentairePrestataire = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Dossier__CABB1DF09F9A6B6B", x => x.DossierID);
                    table.ForeignKey(
                        name: "FK__Dossier__Assuran__4222D4EF",
                        column: x => x.AssuranceID,
                        principalTable: "Assurance",
                        principalColumn: "AssuranceID");
                    table.ForeignKey(
                        name: "FK__Dossier__ClientI__412EB0B6",
                        column: x => x.ClientID,
                        principalTable: "Client",
                        principalColumn: "ClientID");
                    table.ForeignKey(
                        name: "FK__Dossier__Prestat__4316F928",
                        column: x => x.PrestataireID,
                        principalTable: "Prestataire",
                        principalColumn: "PrestataireID");
                });

            migrationBuilder.CreateTable(
                name: "Facture",
                columns: table => new
                {
                    FactureID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierID = table.Column<int>(type: "int", nullable: true),
                    DateFacture = table.Column<DateOnly>(type: "date", nullable: true),
                    MontantHT = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MontantTVA = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    MontantTTC = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facture__511BBA009F004EFE", x => x.FactureID);
                    table.ForeignKey(
                        name: "FK__Facture__Dossier__6EF57B66",
                        column: x => x.DossierID,
                        principalTable: "Dossier",
                        principalColumn: "DossierID");
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueModification",
                columns: table => new
                {
                    HistoriqueID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierID = table.Column<int>(type: "int", nullable: true),
                    ChampModifie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AncienneValeur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NouvelleValeur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    ModifiePar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Historiq__0028F06DE7117746", x => x.HistoriqueID);
                    table.ForeignKey(
                        name: "FK__Historiqu__Dossi__49C3F6B7",
                        column: x => x.DossierID,
                        principalTable: "Dossier",
                        principalColumn: "DossierID");
                });

            migrationBuilder.CreateTable(
                name: "Modification",
                columns: table => new
                {
                    ModificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierID = table.Column<int>(type: "int", nullable: true),
                    DateModification = table.Column<DateTime>(type: "datetime", nullable: true),
                    TypeModification = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UtilisateurID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Modifica__A3FE5A12EC0C9BEC", x => x.ModificationID);
                    table.ForeignKey(
                        name: "FK__Modificat__Dossi__74AE54BC",
                        column: x => x.DossierID,
                        principalTable: "Dossier",
                        principalColumn: "DossierID");
                    table.ForeignKey(
                        name: "FK__Modificat__Utili__75A278F5",
                        column: x => x.UtilisateurID,
                        principalTable: "Utilisateur",
                        principalColumn: "UtilisateurID");
                });

            migrationBuilder.CreateTable(
                name: "PieceJointe",
                columns: table => new
                {
                    PieceJointeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DossierID = table.Column<int>(type: "int", nullable: true),
                    NomFichier = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TypeFichier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateAjout = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PieceJoi__27E19E15C63AFF3E", x => x.PieceJointeID);
                    table.ForeignKey(
                        name: "FK__PieceJoin__Dossi__45F365D3",
                        column: x => x.DossierID,
                        principalTable: "Dossier",
                        principalColumn: "DossierID");
                });

            migrationBuilder.CreateTable(
                name: "Paiement",
                columns: table => new
                {
                    PaiementID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactureID = table.Column<int>(type: "int", nullable: true),
                    DatePaiement = table.Column<DateOnly>(type: "date", nullable: true),
                    Montant = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ModePaiement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Recu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MontantRestant = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Paiement__A8FB0857BBD247F1", x => x.PaiementID);
                    table.ForeignKey(
                        name: "FK__Paiement__Factur__71D1E811",
                        column: x => x.FactureID,
                        principalTable: "Facture",
                        principalColumn: "FactureID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Client_AssuranceID",
                table: "Client",
                column: "AssuranceID");

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_AssuranceID",
                table: "Dossier",
                column: "AssuranceID");

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_ClientID",
                table: "Dossier",
                column: "ClientID");

            migrationBuilder.CreateIndex(
                name: "IX_Dossier_PrestataireID",
                table: "Dossier",
                column: "PrestataireID");

            migrationBuilder.CreateIndex(
                name: "IX_Facture_DossierID",
                table: "Facture",
                column: "DossierID");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueModification_DossierID",
                table: "HistoriqueModification",
                column: "DossierID");

            migrationBuilder.CreateIndex(
                name: "IX_Modification_DossierID",
                table: "Modification",
                column: "DossierID");

            migrationBuilder.CreateIndex(
                name: "IX_Modification_UtilisateurID",
                table: "Modification",
                column: "UtilisateurID");

            migrationBuilder.CreateIndex(
                name: "IX_Paiement_FactureID",
                table: "Paiement",
                column: "FactureID");

            migrationBuilder.CreateIndex(
                name: "IX_PieceJointe_DossierID",
                table: "PieceJointe",
                column: "DossierID");

            migrationBuilder.CreateIndex(
                name: "UQ__Utilisat__A9D10534CA3CCA1E",
                table: "Utilisateur",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoriqueModification");

            migrationBuilder.DropTable(
                name: "Modification");

            migrationBuilder.DropTable(
                name: "Paiement");

            migrationBuilder.DropTable(
                name: "PieceJointe");

            migrationBuilder.DropTable(
                name: "Utilisateur");

            migrationBuilder.DropTable(
                name: "Facture");

            migrationBuilder.DropTable(
                name: "Dossier");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Prestataire");

            migrationBuilder.DropTable(
                name: "Assurance");
        }
    }
}
