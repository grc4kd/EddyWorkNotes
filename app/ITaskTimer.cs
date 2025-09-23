namespace Eddy;

public interface ITaskTimer
{
    TimeSpan Period { get; }
    Task StartAsync();
    void Pause();
    Task ResumeAsync();
    Task CancelAsync();

    // Change TimerCompleted to use Func<Task>
    public Func<Task>? TimerCompleted { get; set; }
}
