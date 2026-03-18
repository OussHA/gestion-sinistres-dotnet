using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Prestataire")]
public partial class Prestataire
{
    [Key]
    public int PrestataireID { get; set; }

    [StringLength(100)]
    public string? Nom { get; set; }

    public string? Services { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TarifNormal { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TarifSoirWeekendFerie { get; set; }

    public string? Agences { get; set; }

    [StringLength(100)]
    public string? Contact { get; set; }

    [StringLength(100)]
    public string? Mail { get; set; }

    [StringLength(200)]
    public string? AdresseSiege { get; set; }

    [StringLength(20)]
    public string? CodePostalSiege { get; set; }

    [StringLength(100)]
    public string? VilleSiege { get; set; }

    [StringLength(200)]
    public string? Horaires { get; set; }

    [StringLength(100)]
    public string? SiteWeb { get; set; }

    [Column(TypeName = "decimal(3, 2)")]
    public decimal? Note { get; set; }

    [InverseProperty("Prestataire")]
    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();
}
