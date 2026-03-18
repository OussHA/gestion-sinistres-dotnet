using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Facture")]
public partial class Facture
{
    [Key]
    public int FactureID { get; set; }

    public int? DossierID { get; set; }

    public DateOnly? DateFacture { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MontantHT { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MontantTVA { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MontantTTC { get; set; }

    [StringLength(50)]
    public string? Statut { get; set; }

    public string? Commentaire { get; set; }

    [ForeignKey("DossierID")]
    [InverseProperty("Factures")]
    public virtual Dossier? Dossier { get; set; }

    [InverseProperty("Facture")]
    public virtual ICollection<Paiement> Paiements { get; set; } = new List<Paiement>();
}
