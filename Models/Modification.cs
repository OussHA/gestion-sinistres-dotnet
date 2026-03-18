using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("Modification")]
public partial class Modification
{
    [Key]
    public int ModificationID { get; set; }

    public int? DossierID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModification { get; set; }

    [StringLength(50)]
    public string? TypeModification { get; set; }

    public string? Description { get; set; }

    public string? Commentaire { get; set; }

    public int? UtilisateurID { get; set; }

    [ForeignKey("DossierID")]
    [InverseProperty("Modifications")]
    public virtual Dossier? Dossier { get; set; }

    [ForeignKey("UtilisateurID")]
    [InverseProperty("Modifications")]
    public virtual Utilisateur? Utilisateur { get; set; }
}
