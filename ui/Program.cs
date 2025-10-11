using Eddy;
using Markdig;
using Microsoft.EntityFrameworkCore;
using ui.Data;
using Microsoft.Data.Sqlite;
using ui.Components;

var builder = WebApplication.CreateBuilder(args);

var conStrBuilder = new SqliteConnectionStringBuilder(
    builder.Configuration.GetConnectionString("EddyWorkNotesContext")
);
var connection = conStrBuilder.ConnectionString;

builder.Services.AddDbContextFactory<EddyWorkNotesContext>(options =>
    options.UseSqlite(connection ?? throw new InvalidOperationException("Connection string 'EddyWorkNotesContext' not found.")));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<NotifierService>();
builder.Services.AddScoped<TaskTimerService>();
builder.Services.AddScoped<MarkdownPipelineBuilder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Initialize database after use static files
app.CreateDbIfNotExists();

// Seed database in development (drops existing data)
if (app.Environment.IsDevelopment())
{
    app.InitializeDb();
}

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
