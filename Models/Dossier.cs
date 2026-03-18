using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Dossier")]
public partial class Dossier
{
    [Key]
    public int DossierID { get; set; }

    public int? ClientID { get; set; }

    public int? AssuranceID { get; set; }

    public int? PrestataireID { get; set; }

    public DateOnly? DateDeclaration { get; set; }

    [StringLength(100)]
    public string? TypeSinistre { get; set; }

    [StringLength(50)]
    public string? Priorite { get; set; }

    [StringLength(50)]
    public string? Etat { get; set; }

    public string? Description { get; set; }

    public DateOnly? DateDebutTravaux { get; set; }

    public DateOnly? DateFinTravaux { get; set; }

    public string? CommentaireISH { get; set; }

    public string? CommentaireAssurance { get; set; }

    public string? CommentairePrestataire { get; set; }

    [ForeignKey("AssuranceID")]
    [InverseProperty("Dossiers")]
    public virtual Assurance? Assurance { get; set; }

    [ForeignKey("ClientID")]
    [InverseProperty("Dossiers")]
    public virtual Client? Client { get; set; }

    [InverseProperty("Dossier")]
    public virtual ICollection<Facture> Factures { get; set; } = new List<Facture>();

    [InverseProperty("Dossier")]
    public virtual ICollection<HistoriqueModification> HistoriqueModifications { get; set; } = new List<HistoriqueModification>();

    [InverseProperty("Dossier")]
    public virtual ICollection<Modification> Modifications { get; set; } = new List<Modification>();

    [InverseProperty("Dossier")]
    public virtual ICollection<PieceJointe> PieceJointes { get; set; } = new List<PieceJointe>();

    [ForeignKey("PrestataireID")]
    [InverseProperty("Dossiers")]
    public virtual Prestataire? Prestataire { get; set; }

    public int UtilisateurID { get; set; }  // Clé étrangère
    public Utilisateur? Utilisateur { get; set; }  // Navigation

    public ICollection<DossierUtilisateur> DossierUtilisateurs { get; set; } = new List<DossierUtilisateur>();
}
