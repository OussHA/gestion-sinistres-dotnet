using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ISH_APP.Models; // ou le namespace où se trouve ta classe PieceJointe


namespace ISH_APP.Models;

[Table("PieceJointe")]
public partial class PieceJointe
{
    [Key]
    public int PieceJointeID { get; set; }

    public int? DossierID { get; set; }

    [StringLength(200)]
    public string? NomFichier { get; set; }

    [StringLength(50)]
    public string? TypeFichier { get; set; }

    public string? CheminFichier { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateAjout { get; set; }

    [Column(TypeName = "varbinary(max)")]
    public byte[]? Contenu { get; set; }

    [ForeignKey("DossierID")]
    [InverseProperty("PieceJointes")]
    public virtual Dossier? Dossier { get; set; }

}
