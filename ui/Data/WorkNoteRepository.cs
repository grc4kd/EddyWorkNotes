using DataEntities;
using DataEntities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ui.Data;

public class WorkNoteRepository : IWorkNoteRepository
{
    private readonly EddyWorkNotesContext _context;

    public WorkNoteRepository(EddyWorkNotesContext context)
    {
        _context = context;
    }

    public async Task<List<WorkNote>> GetAllWorkNotes()
    {
        return await _context.WorkNote.ToListAsync();
    }

    public async Task<List<WorkNote>> GetNotes(int idMin, int idMax)
    {
        return await _context.WorkNote
            .Where(n => n.Id >= idMin && n.Id <= idMax)
            .ToListAsync();
    }
}
