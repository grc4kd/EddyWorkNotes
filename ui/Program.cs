using Eddy;
using Markdig;
using Microsoft.EntityFrameworkCore;
using ui.Data;
using ui.Components;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var connStrName = "EddyWorkNotes";

static NpgsqlConnectionStringBuilder BuildConnectionString(WebApplicationBuilder builder, string connStrName)
{
    var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder(
        builder.Configuration.GetConnectionString(connStrName)
        ?? throw new InvalidOperationException($"Connection string '{connStrName}' not found."))
    {
        // allow overriding the postgres database server host using environment variable
        Host = Environment.GetEnvironmentVariable("EDDY_POSTGRES_HOST") ?? "localhost"
    };
    return npgsqlConnectionStringBuilder;
}

builder.Services.AddDbContextFactory<EddyWorkNotesContext>(options => options.UseNpgsql(BuildConnectionString(builder, connStrName).ConnectionString));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Custom app service dependencies
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<NotifierService>();
builder.Services.AddScoped<TaskTimerService>();
builder.Services.AddScoped<TimerCycleService>();
builder.Services.AddScoped<WorkNoteRepository>();

// Third party library service dependencies
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
