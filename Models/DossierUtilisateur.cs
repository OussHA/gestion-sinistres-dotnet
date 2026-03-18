namespace ISH_APP.Models
{
    public class DossierUtilisateur
    {
        public int DossierID { get; set; }
        public Dossier Dossier { get; set; }

        public int UtilisateurID { get; set; }
        public Utilisateur Utilisateur { get; set; }
    }
}
