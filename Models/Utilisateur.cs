using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Utilisateur")]
[Index("Email", Name = "UQ__Utilisat__A9D10534CA3CCA1E", IsUnique = true)]
public partial class Utilisateur
{
    [Key]
    public int UtilisateurID { get; set; }

    [StringLength(100)]
    public string? Nom { get; set; }

    [StringLength(100)]
    public string? Prenom { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(256)]
    public string? MotDePasseHash { get; set; }

    [StringLength(50)]
    public string? Role { get; set; }

    public byte[]? Logo { get; set; }

    [InverseProperty("Utilisateur")]
    public virtual ICollection<Modification> Modifications { get; set; } = new List<Modification>();

    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();

    public ICollection<DossierUtilisateur> DossierUtilisateurs { get; set; } = new List<DossierUtilisateur>();

}
