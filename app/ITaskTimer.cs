namespace Eddy;

public interface ITaskTimer
{
    TimeSpan Period { get; }
    bool IsRunning { get; }
    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
    public event EventHandler? TimerCompleted;
    public Task StartAsync();
    public void Pause();
    public Task ResumeAsync();
}
