namespace ui.Components.Models;

public record TimerSession(string Message, TimeSpan Period, DateTime CompletedAt, string? WorkNotes);