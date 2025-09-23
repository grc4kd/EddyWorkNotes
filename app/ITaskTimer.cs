namespace Eddy;

public interface ITaskTimer
{
    TimeSpan Period { get; }
    Task StartAsync();
    void Pause();
    Task ResumeAsync();
    Task CancelAsync();
    Func<Task>? TimerCompleted { get; set; }
}
