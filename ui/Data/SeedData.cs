using Eddy.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;

namespace ui.Data;

/// <summary>
/// Seed initial data for the WorkNotes table.
/// </summary>
public class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        // Resolve the DbContext from the service provider
        using var context = new EddyWorkNotesContext(
            serviceProvider.GetRequiredService<
                DbContextOptions<EddyWorkNotesContext>>());
        
        // Defensive null‑check – throws a clear exception if something is wrong
        if (context == null || context.WorkNote == null)
        {
            throw new NullReferenceException(
                "Null YourDbContext or WorkNotes DbSet");
        }

        // Bail out if we already have data – prevents duplicate rows on re‑run
        if (context.WorkNote.Any())
        {
            return;
        }

        // Add a few demo WorkNotes.  Feel free to replace or extend these.
        context.WorkNote.AddRange(
            new WorkNote
            {
                RecordedAtTime = new DateTime(1986, 8, 15, 11, 35, 0),
                Description = "Set up initial project structure and folder hierarchy."
            },
            new WorkNote
            {
                RecordedAtTime = new DateTime(1986, 8, 15, 11, 50, 0),
                Description = "Configured Entity Framework Core with SQL Server provider."
            },
            new WorkNote
            {
                RecordedAtTime = new DateTime(1986, 8, 15, 12, 05, 0),
                Description = "Added migration to create the WorkNotes table."
            },
            new WorkNote
            {
                RecordedAtTime = new DateTime(1986, 8, 15, 12, 20, 0),
                Description = "Implemented the SeedData class to populate the table on startup."
            },
            new WorkNote
            {
                RecordedAtTime = new DateTime(1986, 8, 16, 1, 0, 0),
                Description = "Review and document all work items in this table for future reference."
            });

        // Persist changes to the database
        context.SaveChanges();
    }
}