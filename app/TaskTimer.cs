namespace Eddy;


/// <summary>
/// Defaults to 25 minutes for <see cref="WorkDuration"/>.
/// Defaults to  5 minutes for <see cref="BreakDuration"/>.
/// </summary>
/// <param name="WorkDuration"></param>
/// <param name="BreakDuration"></param>
/// <param name="IsWorkTime"></param>
/// <param name="RemainingTime"></param>
public record TaskTimer(int WorkDuration, int BreakDuration, bool IsWorkTime) : IDisposable
{
    private CancellationTokenSource _cts = new();

    public int WorkDuration { get; set; } = WorkDuration;
    public int BreakDuration { get; set; } = BreakDuration;
    public bool IsRunning { get; set; } = false;
    public bool IsWorkTime { get; set; } = IsWorkTime;
    public double RemainingTime { get; set; }

    public event EventHandler? TimeElapsed;
    public event EventHandler? TimerCompleted;

    private static PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1));

    /// <param name="IsWorkTime"></param>
    public TaskTimer(bool IsWorkTime, int WorkDuration, int BreakDuration) : this(WorkDuration, BreakDuration, IsWorkTime)
    {
        ResetTimer();
    }

    private void ResetTimer()
    {
        _timer = new(TimeSpan.FromSeconds(IsWorkTime switch
        {
            true => WorkDuration,
            false => BreakDuration
        }));

        RemainingTime = _timer.Period.TotalSeconds;
    }

    public TaskTimer(int WorkDuration, int BreakDuration) : this(IsWorkTime: true, WorkDuration, BreakDuration)
    {
        
    }

    public virtual void OnTimeElapsed(EventArgs e)
    {
        TimeElapsed?.Invoke(this, e);
    }

    public virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke(this, e);
    }

    public async Task StartAsync()
    {
        ResetTimer();

        if (_cts.IsCancellationRequested)
            TogglePause();

        await UpdateTimeAsync(_cts.Token);
    }

    public void TogglePause()
    {
        _cts?.Cancel();
        IsRunning = !IsRunning;
    }

    private async Task UpdateTimeAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        // stop and complete the timer after break is over
        IsRunning = IsWorkTime;

        await _timer.WaitForNextTickAsync(_cts.Token);

        // switch to break time, then reset timer
        IsWorkTime = !IsWorkTime;
        ResetTimer();
    }

    public void Dispose()
    {
        TimerCompleted?.Method.Invoke(this, []);
        GC.SuppressFinalize(this);
    }
}