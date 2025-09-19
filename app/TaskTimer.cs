using Microsoft.Extensions.Logging;

namespace Eddy;

/// <summary>
/// The TaskTimer record type encapsulates the behavior of a work/break timer.
/// After the initial work period, a break period is started, then a new work period is started again.
/// The timer continues like this indefinetely unless interrupted by the user or an exception.
/// </summary>
/// <param name="WorkMinutes">The length of the work cycle in minutes.</param>
/// <param name="BreakMinutes">The length of the break cycle in minutes.</param>
/// <param name="IsWorkTime">Start with work time? Exposes additional state so method can be called starting with break time.</param>
public record TaskTimer(int WorkMinutes, int BreakMinutes, bool IsWorkTime) : IDisposable
{
    private static ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger logger = factory.CreateLogger("TaskTimer");

    private readonly CancellationTokenSource _cts = new();
    private DateTimeOffset lastEventTimeUtc = DateTimeOffset.UtcNow;
    private PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1));

    public int WorkMinutes { get; } = WorkMinutes;
    public int BreakMinutes { get; } = BreakMinutes;
    public bool IsRunning { get; set; } = false;
    public bool IsWorkTime { get; set; } = IsWorkTime;
    public double RemainingTime { get; set; }

    public event EventHandler? TimerCompleted;

    public TaskTimer(int WorkMinutes, int BreakMinutes) : this(WorkMinutes, BreakMinutes, IsWorkTime: true)
    {
        
        logger.LogInformation($"Constructed {nameof(TaskTimer)} with {nameof(TaskTimer.WorkMinutes)}: {WorkMinutes}, {nameof(TaskTimer.BreakMinutes)}: {BreakMinutes}");
    }

    private void ResetTimer()
    {
        lastEventTimeUtc = DateTimeOffset.UtcNow;

        if (IsWorkTime)
        {
            TimeSpan initialTimeSpan = WorkMinutes > 0 ?
                TimeSpan.FromMinutes(WorkMinutes) :
                TimeSpan.FromMilliseconds(1);

            _timer = new(initialTimeSpan);
        }
        else
        {
            TimeSpan initialTimeSpan = BreakMinutes > 0 ?
                TimeSpan.FromMinutes(BreakMinutes) :
                TimeSpan.FromMilliseconds(1);

            _timer = new(initialTimeSpan);
        }

        RemainingTime = _timer.Period.TotalSeconds;
        IsRunning = true;
    }

    public async Task Cancel()
    {
        if (!_cts.IsCancellationRequested)
        {
            await _cts.CancelAsync();
        }

        OnTimerCompleted(EventArgs.Empty);
    }

    public virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke(this, e);
    }

    public async Task StartAsync()
    {
        ResetTimer();

        // If cancellation was requested, skip clock time updates
        if (!_cts.IsCancellationRequested)
        {
            await UpdateTimeAsync(_cts.Token);
        }
    }

    public void TogglePause()
    {
        if (IsRunning)
        {
            double elapsedTime = DateTimeOffset.UtcNow.Subtract(lastEventTimeUtc).TotalSeconds;
            RemainingTime -= elapsedTime;
        }

        IsRunning = !IsRunning;
        lastEventTimeUtc = DateTimeOffset.UtcNow;

        string logPrefix = IsRunning ? "Resuming" : "Pausing";
        logger.LogInformation($"{logPrefix} timer with {nameof(RemainingTime)}: {RemainingTime} seconds left. Last Event Time (UTC): [{lastEventTimeUtc}].");
        
        _timer.Dispose();
        OnTimerCompleted(EventArgs.Empty);
    }

    private async Task UpdateTimeAsync(CancellationToken cancellationToken)
    {
        if (!cancellationToken.IsCancellationRequested)
        {
            await _timer.WaitForNextTickAsync(cancellationToken);
        }

        if (IsRunning)
        {
            // Switch to break time, then reset timer
            IsWorkTime = !IsWorkTime;
            ResetTimer();
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
