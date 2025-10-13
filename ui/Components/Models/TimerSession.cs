namespace ui.Components.Models;

public record TimerSession(string Message, DateTime CompletedAtUtc, string? WorkNotes)
{
    public TimerSession(DataEntities.WorkNote workNote) : this("Work", workNote.RecordedAtTimeUtc.ToUniversalTime(), workNote.Description) { }

    private DateTime _completedAtUtc = CompletedAtUtc;
    public DateTime CompletedAtLocaltime = CompletedAtUtc.ToLocalTime();
}