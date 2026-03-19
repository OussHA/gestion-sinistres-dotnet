using ISH_APP.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISH_APP.Pages.DossierPages
{
    public class FacturationModel : PageModel
    {
        private readonly ISH_APP.Data.ApplicationDbContext _context;

        public FacturationModel(ISH_APP.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int DossierID { get; set; }

        public Dossier? Dossier { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class LigneFactureModel
        {
            [Required]
            public string Description { get; set; }

            [Required]
            public decimal Quantite { get; set; }

            [Required]
            [ModelBinder(BinderType = typeof(DecimalModelBinder))]  // 👈 ajout ici
            public decimal PrixUnitaire { get; set; }
        }

        public class DecimalModelBinder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                if (valueProviderResult != ValueProviderResult.None)
                {
                    bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                    var value = valueProviderResult.FirstValue;

                    if (!string.IsNullOrEmpty(value))
                    {
                        // Remplace les virgules par des points pour uniformiser
                        value = value.Replace(",", ".");

                        if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                        {
                            bindingContext.Result = ModelBindingResult.Success(parsedValue);
                            return Task.CompletedTask;
                        }
                    }
                }

                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
        }


        public class InputModel
        {
            [Required]
            public string Reference { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }

            [Required]
            [Range(0, 100)]
            public decimal TauxTVA { get; set; }

            [Required]
            public string TypeDocument { get; set; } // "Facture" ou "Devis"

            public List<LigneFactureModel> Lignes { get; set; } = new();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Dossier = await _context.Dossiers
                .Include(d => d.Assurance)
                .Include(d => d.Prestataire)
                .Include(d => d.Client)
                .FirstOrDefaultAsync(d => d.DossierID == DossierID);

            if (Dossier == null)
                return NotFound();

            Input = new InputModel
            {
                Date = DateTime.Today,
                TauxTVA = 20

                ,
                TypeDocument = "Devis",
                Lignes = new List<LigneFactureModel> {
                    new LigneFactureModel { Description = "", Quantite = 1, PrixUnitaire = 0 }
                }
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            Dossier = await _context.Dossiers
                .Include(d => d.Assurance)
                .Include(d => d.Prestataire)
                .Include(d => d.Client)
                .FirstOrDefaultAsync(d => d.DossierID == DossierID);

            if (Dossier == null)
                return NotFound();

            decimal totalHT = Input.Lignes.Sum(l => l.Quantite * l.PrixUnitaire);
            decimal montantTVA = totalHT * Input.TauxTVA / 100;
            decimal totalTTC = totalHT + montantTVA;

            byte[] pdfBytes;
            using (var ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 60, 60);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // ✅ Définir polices
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);

                // ✅ Ajouter le logo (optionnel)
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Photos", "logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoPath);
                    logo.ScaleAbsoluteWidth(100);
                    logo.ScaleAbsoluteHeight(50);
                    logo.Alignment = Element.ALIGN_LEFT;
                    doc.Add(logo);
                }

                // ✅ Création d’un tableau à 2 colonnes pour Prestataire & Client
                PdfPTable headerTable = new PdfPTable(2);
                headerTable.WidthPercentage = 100;
                headerTable.SetWidths(new float[] { 50, 50 });

                // ✅ Bloc Prestataire
                PdfPTable prestataireTable = new PdfPTable(1);
                prestataireTable.AddCell(new PdfPCell(new Phrase(Dossier.Prestataire?.Nom ?? "-", boldFont)) { Border = 0 });
                prestataireTable.AddCell(new PdfPCell(new Phrase($"Adresse : {Dossier.Prestataire?.AdresseSiege ?? "-"}", normalFont)) { Border = 0 });
                prestataireTable.AddCell(new PdfPCell(new Phrase($"Ville : {Dossier.Prestataire?.VilleSiege ?? "-"}", normalFont)) { Border = 0 });
                prestataireTable.AddCell(new PdfPCell(new Phrase($"Téléphone : {Dossier.Prestataire?.Contact ?? "-"}", normalFont)) { Border = 0 });
                prestataireTable.AddCell(new PdfPCell(new Phrase($"Email : {Dossier.Prestataire?.Mail ?? "-"}", normalFont)) { Border = 0 });

                // ✅ Bloc Client
                PdfPTable clientTable = new PdfPTable(1);
                clientTable.AddCell(new PdfPCell(new Phrase("CLIENT", boldFont)) { Border = 0 });
                clientTable.AddCell(new PdfPCell(new Phrase($"{Dossier.Client?.Nom} {Dossier.Client?.Prenom}", normalFont)) { Border = 0 });
                clientTable.AddCell(new PdfPCell(new Phrase($"Adresse : {Dossier.Client?.Adresse ?? "-"}", normalFont)) { Border = 0 });
                clientTable.AddCell(new PdfPCell(new Phrase($"Ville : {Dossier.Client?.Ville ?? "-"}", normalFont)) { Border = 0 });
                clientTable.AddCell(new PdfPCell(new Phrase($"Téléphone : {Dossier.Client?.Telephone ?? "-"}", normalFont)) { Border = 0 });

                headerTable.AddCell(new PdfPCell(prestataireTable) { Border = 0 });
                headerTable.AddCell(new PdfPCell(clientTable) { Border = 0 });
                doc.Add(headerTable);

                doc.Add(new Paragraph("\n"));

                // ✅ Info Devis
                string devisNum = $"DEV-{DateTime.UtcNow:yyyy-MMdd}-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 70, 30 });

                PdfPCell leftCell = new PdfPCell();
                leftCell.AddElement(new Phrase($"DEVIS N° {devisNum}", boldFont));
                leftCell.AddElement(new Phrase("Valable 3 mois", normalFont));
                leftCell.Border = 0;

                PdfPCell rightCell = new PdfPCell(new Phrase($"Date : {DateTime.UtcNow:dd/MM/yyyy}", normalFont));
                rightCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                rightCell.Border = 0;

                infoTable.AddCell(leftCell);
                infoTable.AddCell(rightCell);
                doc.Add(infoTable);

                doc.Add(new Paragraph("\n"));

                // ✅ Tableau des articles moderne
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;
                table.SetWidths(new float[] { 8, 30, 10, 15, 10, 15 });

                // ✅ En-tête stylée
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                var cellColor = new BaseColor(0, 102, 204);

                string[] headers = { "N°", "Désignation", "Qté", "Prix U. HT", "% TVA", "Total HT" };
                foreach (var h in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(h, headerFont))
                    {
                        BackgroundColor = cellColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 8,
                        Border = Rectangle.NO_BORDER
                    };
                    table.AddCell(cell);
                }

                // ✅ Lignes modernes sans bordures avec fond clair alterné
                
                int index = 1;
                bool alternate = false;

                foreach (var ligne in Input.Lignes)
                {
                    BaseColor bgColor = alternate ? new BaseColor(245, 245, 245) : BaseColor.WHITE; // Alternance gris clair
                    alternate = !alternate;

                    PdfPCell[] rowCells = new PdfPCell[]
                    {
        new PdfPCell(new Phrase(index.ToString(), normalFont)),
        new PdfPCell(new Phrase(ligne.Description, normalFont)),
        new PdfPCell(new Phrase(ligne.Quantite.ToString("N2"), normalFont)),
        new PdfPCell(new Phrase($"{ligne.PrixUnitaire:N2} €", normalFont)),
        new PdfPCell(new Phrase($"{Input.TauxTVA}%", normalFont)),
        new PdfPCell(new Phrase($"{(ligne.Quantite * ligne.PrixUnitaire):N2} €", normalFont))
                    };

                    foreach (var cell in rowCells)
                    {
                        cell.Border = Rectangle.NO_BORDER; // Pas de bordures
                        cell.Padding = 6; // Espacement intérieur
                        cell.BackgroundColor = bgColor; // Fond alterné
                    }

                    foreach (var cell in rowCells)
                        table.AddCell(cell);

                    index++;
                }

                doc.Add(table);

                doc.Add(new Paragraph("\n"));

                // ✅ Tableau Total
                PdfPTable totalTable = new PdfPTable(2);
                totalTable.WidthPercentage = 40;
                totalTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                void AddTotalRow(string label, string value, bool bold = false)
                {
                    totalTable.AddCell(new PdfPCell(new Phrase(label, bold ? boldFont : normalFont)) { Border = Rectangle.NO_BORDER });
                    totalTable.AddCell(new PdfPCell(new Phrase(value, bold ? boldFont : normalFont)) { Border = Rectangle.NO_BORDER, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                AddTotalRow("Total HT", $"{totalHT:N2} €");
                AddTotalRow($"TVA ({Input.TauxTVA}%)", $"{montantTVA:N2} €");
                AddTotalRow("Total TTC", $"{totalTTC:N2} €", true);

                doc.Add(totalTable);

                doc.Add(new Paragraph("\n\n"));

                // ✅ Signatures
                PdfPTable signatureTable = new PdfPTable(2);
                signatureTable.WidthPercentage = 100;
                signatureTable.SetWidths(new float[] { 50, 50 });

                signatureTable.AddCell(new PdfPCell(new Phrase("Signature Client", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });
                signatureTable.AddCell(new PdfPCell(new Phrase("Signature Prestataire", normalFont)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });

                doc.Add(signatureTable);

                doc.Close();
                pdfBytes = ms.ToArray();
            }


            var pieceJointe = new PieceJointe
            {
                DossierID = DossierID,
                NomFichier = $"DEV_{DateTime.UtcNow:yyyyMMddHHmmss}_{DossierID}.pdf",
                TypeFichier = "application/pdf",
                Contenu = pdfBytes,
                DateAjout = DateTime.UtcNow
            };

            _context.PiecesJointes.Add(pieceJointe);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Devis généré avec succès.";
            return RedirectToPage("/DossierPages/Details", new { id = DossierID });
        }

    }
}
