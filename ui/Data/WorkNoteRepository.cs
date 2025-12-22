using DataEntities;
using DataEntities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ui.Data;

public class WorkNoteRepository(EddyWorkNotesContext context) : IWorkNoteRepository
{
    public async Task<List<WorkNote>> GetWorkNotesSince(DateTime time)
    {
        return await context.WorkNote
            .Where(w => w.RecordedAtTimeUtc >= time)
            .OrderByDescending(w => w.RecordedAtTimeUtc)
            .ToListAsync();
    }
}