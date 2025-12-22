namespace ui.Components.Models;

public record TimerSession(string Message, DateTime CompletedAtUtc, string? WorkNotes)
{
    public TimerSession(DataEntities.WorkNote workNote) : this("Work", workNote.RecordedAtTimeUtc.ToUniversalTime(), workNote.Description) { }

    public DateTime CompletedAtLocaltime { get; } = CompletedAtUtc.ToLocalTime();
}