namespace DataEntities.Interfaces;

public interface IWorkNoteRepository
{
    Task<List<WorkNote>> GetWorkNotesSince(DateTime time);
}