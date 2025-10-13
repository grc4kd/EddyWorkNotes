using DataEntities;

namespace ui.Data;

/// <summary>
/// Seed initial data for the WorkNotes table.
/// </summary>
public class SeedData
{
    public static void Initialize(EddyWorkNotesContext context)
    {
        // Abort if notes already exist by default.
        if (context.WorkNote.Any())
        {
            return;
        }
        
        // Add a few demo WorkNotes. Feel free to replace or extend these.
        context.UpdateRange(new List<WorkNote>{
            new() {
                RecordedAtTimeUtc = new DateTime(1986, 8, 15, 11, 35, 0).ToUniversalTime(),
                Description = "Set up initial project structure and folder hierarchy."
            },
            new() {
                RecordedAtTimeUtc = new DateTime(1986, 8, 15, 11, 50, 0).ToUniversalTime(),
                Description = "Configured Entity Framework Core with SQL Server provider."
            },
            new() {
                RecordedAtTimeUtc = new DateTime(1986, 8, 15, 12, 05, 0).ToUniversalTime(),
                Description = "Added migration to create the WorkNotes table."
            },
            new() {
                RecordedAtTimeUtc = new DateTime(1986, 8, 15, 12, 20, 0).ToUniversalTime(),
                Description = "Implemented the SeedData class to populate the table on startup."
            },
            new() {
                RecordedAtTimeUtc = new DateTime(1986, 8, 16, 1, 0, 0).ToUniversalTime(),
                Description = "Review and document all work items in this table for future reference."
            } });

        // Persist changes to the database
        context.SaveChanges();
    }
}