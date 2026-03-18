using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Assurance")]
public partial class Assurance
{
    [Key]
    public int AssuranceID { get; set; }

    [StringLength(100)]
    public string? Nom { get; set; }

    public string? Services { get; set; }

    [StringLength(100)]
    public string? Contact { get; set; }

    [StringLength(100)]
    public string? Mail { get; set; }

    [StringLength(200)]
    public string? Adresse { get; set; }

    [StringLength(20)]
    public string? CodePostal { get; set; }

    [StringLength(100)]
    public string? Ville { get; set; }

    [StringLength(100)]
    public string? SiteWeb { get; set; }

    [InverseProperty("Assurance")]
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    [InverseProperty("Assurance")]
    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();
}
