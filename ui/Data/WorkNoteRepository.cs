using DataEntities;
using DataEntities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ui.Data;

public class WorkNoteRepository(EddyWorkNotesContext context) : IWorkNoteRepository
{
    private readonly EddyWorkNotesContext _context = context;

    public async Task<List<WorkNote>> GetAllWorkNotes()
    {
        return await _context.WorkNote.ToListAsync();
    }

    public async Task<List<WorkNote>> GetWorkNotesSince(DateTime time)
    {
        return await _context.WorkNote
                .Where(w =>  w.RecordedAtTimeUtc >= time)
                .OrderByDescending(w => w.RecordedAtTimeUtc)
                .ToListAsync();
    }

    public async Task<List<WorkNote>> GetNotes(int idMin, int idMax)
    {
        return await _context.WorkNote
            .Where(n => n.Id >= idMin && n.Id <= idMax)
            .ToListAsync();
    }
}
