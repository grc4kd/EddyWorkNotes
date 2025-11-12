namespace DataEntities.Interfaces
{
    public interface IWorkNoteRepository
    {
        Task<List<WorkNote>> GetAllWorkNotes();
        Task<List<WorkNote>> GetWorkNotesSince(DateTime time);
        Task<List<WorkNote>> GetNotes(int idMin, int idMax);
    }
}