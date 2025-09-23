namespace Eddy;

public interface ITaskTimer
{
    TimeSpan Period { get; }
    Task StartAsync();
    void Pause();
    Task ResumeAsync();
    Task CancelAsync();
    public Func<Task>? TimerCompleted { get; set; }
}
