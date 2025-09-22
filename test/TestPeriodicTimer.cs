using Eddy;
namespace test;

public class TestPeriodicTimer(TimeSpan period) : ITaskTimer
{
    private readonly CancellationTokenSource _cts = new();
    public TimeSpan Period { get; } = period;

    public event EventHandler? TimerCompleted;

    protected virtual void OnTimerCompleted(EventArgs e) {
        TimerCompleted?.Invoke(this, e);
    }

    public async Task CancelAsync()
    {
        await _cts.CancelAsync();
    }

    public async Task StartAsync()
    {
        // register a fake cancellation that executes when the cancellation is requested
        _cts.Token.Register(() => Task.FromCanceled(_cts.Token));

        // process expected event handlers
        OnTimerCompleted(EventArgs.Empty);

        // return a task after the minimum delay
        await Task.Delay(1);
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

    public Task ResumeAsync()
    {
        return Task.CompletedTask;
    }
}
