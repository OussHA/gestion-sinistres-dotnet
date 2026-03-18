using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Paiement")]
public partial class Paiement
{
    [Key]
    public int PaiementID { get; set; }

    public int? FactureID { get; set; }

    public DateOnly? DatePaiement { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Montant { get; set; }

    [StringLength(50)]
    public string? ModePaiement { get; set; }

    [StringLength(200)]
    public string? Recu { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MontantRestant { get; set; }

    [ForeignKey("FactureID")]
    [InverseProperty("Paiements")]
    public virtual Facture? Facture { get; set; }
}
