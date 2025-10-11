using DataEntities;
using Microsoft.EntityFrameworkCore;

namespace ui.Data;

/// <summary>
/// Seed initial data for the WorkNotes table.
/// </summary>
public class SeedData
{
    public static void Initialize(EddyWorkNotesContext context)
    {
        // Drop all existing notes each time initialize is called.
        if (context.WorkNote.Any())
        {
            context.WorkNote.ExecuteDelete();
        }
        
        // Add a few demo WorkNotes. Feel free to replace or extend these.
        context.UpdateRange(new List<WorkNote>{
            new() {
                RecordedAtTime = new DateTime(1986, 8, 15, 11, 35, 0),
                Description = "Set up initial project structure and folder hierarchy."
            },
            new() {
                RecordedAtTime = new DateTime(1986, 8, 15, 11, 50, 0),
                Description = "Configured Entity Framework Core with SQL Server provider."
            },
            new() {
                RecordedAtTime = new DateTime(1986, 8, 15, 12, 05, 0),
                Description = "Added migration to create the WorkNotes table."
            },
            new() {
                RecordedAtTime = new DateTime(1986, 8, 15, 12, 20, 0),
                Description = "Implemented the SeedData class to populate the table on startup."
            },
            new() {
                RecordedAtTime = new DateTime(1986, 8, 16, 1, 0, 0),
                Description = "Review and document all work items in this table for future reference."
            } });

        // Persist changes to the database
        context.SaveChanges();
    }
}