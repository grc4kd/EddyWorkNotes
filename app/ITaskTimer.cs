namespace Eddy;

public interface ITaskTimer
{
    TimeSpan Period { get; }
    public Task StartAsync();
    public void Pause();
    public Task ResumeAsync();
}
