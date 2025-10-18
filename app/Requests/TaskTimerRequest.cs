namespace Eddy.Requests;

public record TaskTimerRequest(TimeSpan Duration, string Phase = "Work")
{
    
}