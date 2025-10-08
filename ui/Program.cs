using Eddy;
using Markdig;
using ui.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ui.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<EddyWorkNotesContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EddyWorkNotesContext") ?? throw new InvalidOperationException("Connection string 'EddyWorkNotesContext' not found.")));

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<NotifierService>();
builder.Services.AddScoped<TaskTimerService>();
builder.Services.AddSingleton<MarkdownPipelineBuilder>();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
