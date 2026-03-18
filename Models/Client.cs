using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Client")]
public partial class Client
{
    [Key]
    public int ClientID { get; set; }

    [StringLength(100)]
    public string? Nom { get; set; }

    [StringLength(100)]
    public string? Prenom { get; set; }

    public DateOnly? DateNaissance { get; set; }

    [StringLength(200)]
    public string? Adresse { get; set; }

    [StringLength(20)]
    public string? CodePostal { get; set; }

    [StringLength(100)]
    public string? Ville { get; set; }

    public int? AssuranceID { get; set; }

    [StringLength(100)]
    public string? TypeAssurance { get; set; }

    [ForeignKey("AssuranceID")]
    [InverseProperty("Clients")]
    public virtual Assurance? Assurance { get; set; }

    [InverseProperty("Client")]
    public virtual ICollection<Dossier> Dossiers { get; set; } = new List<Dossier>();

    public string? Telephone { get; set; }
    public string? Mail { get; set; }

}
