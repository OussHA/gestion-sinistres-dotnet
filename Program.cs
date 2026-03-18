
using ISH_APP.Data;
using ISH_APP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ajout Razor Pages
builder.Services.AddRazorPages();
builder.Services.AddSession(); // se connecter à une session

// Ajouter le DbContext avec la chaîne de connexion
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/AuthPages/Login"; // Chemin vers ta page de connexion
    });

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

// Mail
builder.Services.AddTransient<EmailSender>();


var app = builder.Build();

app.UseSession(); // activer la session

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



// Routes Razor Pages
app.MapRazorPages();

app.MapGet("/", (HttpContext context) =>
{
    // Si l'utilisateur est déjà connecté (session "UtilisateurID" existe)
    if (context.Session.GetInt32("UtilisateurID") != null)
    {
        return Results.Redirect("/Index"); // Rediriger vers l'Index si l'utilisateur est connecté
    }

    // Sinon, rediriger vers la page de login
    return Results.Redirect("/AuthPages/Login");
});


app.Run();
