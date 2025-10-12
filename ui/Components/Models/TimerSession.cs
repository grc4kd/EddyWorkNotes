namespace ui.Components.Models;

public record TimerSession(string Message, TimeSpan Period, string? WorkNotes)
{
    private DateTime CompletedAtUtc = DateTime.UtcNow;
    public DateTime CompletedAt
    {
        get
        {
            return CompletedAtUtc;
        }
        set
        {
            CompletedAtUtc = value.ToUniversalTime();
        }
    }
}