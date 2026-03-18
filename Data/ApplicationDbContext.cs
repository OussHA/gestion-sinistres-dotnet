using System;
using System.Collections.Generic;
using ISH_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace ISH_APP.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PieceJointe> PiecesJointes { get; set; }


    public virtual DbSet<Assurance> Assurances { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Dossier> Dossiers { get; set; }


    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<HistoriqueModification> HistoriqueModifications { get; set; }

    public virtual DbSet<Modification> Modifications { get; set; }

    public virtual DbSet<Paiement> Paiements { get; set; }

    public virtual DbSet<PieceJointe> PieceJointes { get; set; }

    public virtual DbSet<Prestataire> Prestataires { get; set; }

    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assurance>(entity =>
        {
            entity.HasKey(e => e.AssuranceID).HasName("PK__Assuranc__CB6737D6B5B8C069");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientID).HasName("PK__Client__E67E1A04EAA5A29F");

            entity.HasOne(d => d.Assurance).WithMany(p => p.Clients).HasConstraintName("FK__Client__Assuranc__3E52440B");
        });

        modelBuilder.Entity<Dossier>(entity =>
        {
            entity.HasKey(e => e.DossierID).HasName("PK__Dossier__CABB1DF09F9A6B6B");

            entity.HasOne(d => d.Assurance).WithMany(p => p.Dossiers).HasConstraintName("FK__Dossier__Assuran__4222D4EF");

            entity.HasOne(d => d.Client).WithMany(p => p.Dossiers).HasConstraintName("FK__Dossier__ClientI__412EB0B6");

            entity.HasOne(d => d.Prestataire).WithMany(p => p.Dossiers).HasConstraintName("FK__Dossier__Prestat__4316F928");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.HasKey(e => e.FactureID).HasName("PK__Facture__511BBA009F004EFE");

            entity.HasOne(d => d.Dossier).WithMany(p => p.Factures).HasConstraintName("FK__Facture__Dossier__6EF57B66");
        });

        modelBuilder.Entity<HistoriqueModification>(entity =>
        {
            entity.HasKey(e => e.HistoriqueID).HasName("PK__Historiq__0028F06DE7117746");

            entity.Property(e => e.DateModification).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Dossier)
                .WithMany(p => p.HistoriqueModifications)
                .HasForeignKey(d => d.DossierID)
                .HasConstraintName("FK__Historiqu__Dossi__49C3F6B7")
                .OnDelete(DeleteBehavior.Cascade); // ✅ Ajoute cette ligne
        });


        modelBuilder.Entity<Modification>(entity =>
        {
            entity.HasKey(e => e.ModificationID).HasName("PK__Modifica__A3FE5A12EC0C9BEC");

            entity.HasOne(d => d.Dossier).WithMany(p => p.Modifications).HasConstraintName("FK__Modificat__Dossi__74AE54BC");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Modifications).HasConstraintName("FK__Modificat__Utili__75A278F5");
        });

        modelBuilder.Entity<Paiement>(entity =>
        {
            entity.HasKey(e => e.PaiementID).HasName("PK__Paiement__A8FB0857BBD247F1");

            entity.HasOne(d => d.Facture).WithMany(p => p.Paiements).HasConstraintName("FK__Paiement__Factur__71D1E811");
        });

        modelBuilder.Entity<PieceJointe>(entity =>
        {
            entity.HasKey(e => e.PieceJointeID).HasName("PK__PieceJoi__27E19E15C63AFF3E");

            entity.Property(e => e.DateAjout).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Dossier).WithMany(p => p.PieceJointes).HasConstraintName("FK__PieceJoin__Dossi__45F365D3");
        });

        modelBuilder.Entity<Prestataire>(entity =>
        {
            entity.HasKey(e => e.PrestataireID).HasName("PK__Prestata__0CB38DF24A9706F3");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurID).HasName("PK__Utilisat__6CB6AE1FE8FEDB5B");
        });

        modelBuilder.Entity<DossierUtilisateur>()
    .HasKey(du => new { du.DossierID, du.UtilisateurID });

        modelBuilder.Entity<DossierUtilisateur>()
            .HasOne(du => du.Dossier)
            .WithMany(d => d.DossierUtilisateurs)
            .HasForeignKey(du => du.DossierID);

        modelBuilder.Entity<DossierUtilisateur>()
            .HasOne(du => du.Utilisateur)
            .WithMany(u => u.DossierUtilisateurs)
            .HasForeignKey(du => du.UtilisateurID);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<DossierUtilisateur> DossierUtilisateurs { get; set; }

}
