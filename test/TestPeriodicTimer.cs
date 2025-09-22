using Eddy;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
namespace test;

public class TestPeriodicTimer(TimeSpan period) : ITaskTimer
{
    public TimeSpan Period { get; } = period;

    public event EventHandler? TimerCompleted;

    private readonly CancellationTokenSource _cts = new();

    public async Task CancelAsync()
    {
        await _cts.CancelAsync();
    }

    public Task StartAsync()
    {
        _cts.Token.Register(() => Task.FromCanceled(_cts.Token));
        return Task.CompletedTask;
    }

    public static async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        // Immediately complete the timer for testing purposes
        return await ValueTask.FromResult(true);
    }

    public void Pause()
    {
        return;
    }

    Task ITaskTimer.ResumeAsync()
    {
        return Task.CompletedTask;
    }
}
