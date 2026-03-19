using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Models;

[Table("HistoriqueModification")]
public partial class HistoriqueModification
{
    [Key]
    public int HistoriqueID { get; set; }

    public int? DossierID { get; set; }

    [StringLength(100)]
    public string? ChampModifie { get; set; }

    public string? AncienneValeur { get; set; }

    public string? NouvelleValeur { get; set; }

    
    public DateTime? DateModification { get; set; }

    [StringLength(100)]
    public string? ModifiePar { get; set; }

    [ForeignKey("DossierID")]
    [InverseProperty("HistoriqueModifications")]
    public virtual Dossier? Dossier { get; set; }
}
