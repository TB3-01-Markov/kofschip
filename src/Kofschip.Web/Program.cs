using Kofschip.Core.Conjugation;
using Kofschip.Core.Training;
using Kofschip.Data;
using Kofschip.Web.Components;
using Kofschip.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("Kofschip")
    ?? throw new InvalidOperationException("Connection string 'Kofschip' is missing.");
builder.Services.AddKofschipData(connectionString);

builder.Services.AddSingleton<ConjugationEngine>();
builder.Services.AddScoped<TrainingSessionFactory>();

// Settings and progress live in the visitor's browser, so no account is needed.
builder.Services.AddScoped<BrowserStorage>();
builder.Services.AddScoped<SettingsStore>();
builder.Services.AddScoped<ProgressStore>();

var app = builder.Build();

// Apply migrations and add the seed verbs before the first request.
await app.Services.InitializeKofschipDatabaseAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
