namespace test;

public class TestPeriodicTimer(TimeSpan period)
{
    private readonly CancellationTokenSource _cts = new();
    public TimeSpan Period { get; } = period;

    private bool timerCompleted = false;

    public Func<Task>? TimerCompleted
    {
        get => () => Task.FromResult(timerCompleted);
        set
        {
            if (!_cts.IsCancellationRequested)
            {
                var task = value?.Invoke();
                task?.Wait();
            }
        }
    }

    protected virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke();
    }

    public async Task CancelAsync()
    {
        await _cts.CancelAsync();
    }

    public async Task StartAsync()
    {
        if (!_cts.IsCancellationRequested)
        {
            // process expected event handlers
            OnTimerCompleted(EventArgs.Empty);

            // return a task after the minimum delay
            await Task.Delay(1);
        }

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
