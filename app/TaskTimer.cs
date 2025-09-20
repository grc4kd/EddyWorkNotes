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
public record TaskTimer(int WorkMinutes, int BreakMinutes, bool IsWorkTime)
{
    private static readonly ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
    private readonly ILogger logger = factory.CreateLogger("TaskTimer");

    private readonly CancellationTokenSource _cts = new();
    private DateTimeOffset lastEventTimeUtc = DateTimeOffset.UtcNow;
    private PeriodicTimer? _timer;

    /// <summary>
    /// Gets the length of the work cycle in minutes.
    /// </summary>
    public int WorkMinutes { get; } = WorkMinutes;

    /// <summary>
    /// Gets the length of the break cycle in minutes.
    /// </summary>
    public int BreakMinutes { get; } = BreakMinutes;

    /// <summary>
    /// Indicates whether the timer is currently running.
    /// </summary>
    public bool IsRunning { get; set; } = false;

    /// <summary>
    /// Indicates whether the current period is work time (true) or break time (false).
    /// </summary>
    public bool IsWorkTime { get; set; } = IsWorkTime;

    /// <summary>
    /// Gets or sets the remaining time in seconds for the current period.
    /// </summary>
    public double? RemainingSeconds { get; set; }

    /// <summary>
    /// Event raised when the timer completes or is cancelled.
    /// </summary>
    public event EventHandler? TimerCompleted;

    /// <summary>
    /// Initializes a new instance of TaskTimer with default work time state.
    /// </summary>
    /// <param name="WorkMinutes">The length of the work cycle in minutes.</param>
    /// <param name="BreakMinutes">The length of the break cycle in minutes.</param>
    public TaskTimer(int WorkMinutes, int BreakMinutes) : this(WorkMinutes, BreakMinutes, IsWorkTime: true)
    {
        RemainingSeconds = IsWorkTime ? WorkMinutes * 60 : BreakMinutes * 60;
    }

    /// <summary>
    /// Cancels the current timer operation and raises the TimerCompleted event.
    /// </summary>
    /// <returns>A task that represents the asynchronous cancellation.</returns>
    public async Task CancelAsync()
    {
        if (!_cts.IsCancellationRequested)
        {
            await _cts.CancelAsync();
        }

        _timer?.Dispose();
        OnTimerCompleted(EventArgs.Empty);
    }

    /// <summary>
    /// Raises the TimerCompleted event.
    /// </summary>
    /// <param name="e">EventArgs to pass with the event. Empty EventArgs will still raise a normal event.</param>
    public virtual void OnTimerCompleted(EventArgs e)
    {
        TimerCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Starts or restarts the timer.
    /// If cancellation was requested, skips clock time updates.
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// </summary>
    public async Task StartAsync()
    {
        lastEventTimeUtc = DateTimeOffset.UtcNow;

        int setTimeMinutes = 0;

        switch (IsWorkTime)
        {
            case true:
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(WorkMinutes);

                setTimeMinutes = WorkMinutes;
                break;
            case false:
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(BreakMinutes);

                setTimeMinutes = BreakMinutes;
                break;
        }

        _timer = new(TimeSpan.FromMinutes(setTimeMinutes));
        RemainingSeconds = _timer.Period.TotalSeconds;
        IsRunning = true;

        logger.LogInformation("Starting timer for {TimerPeriod} seconds at {LastEventTimeUtc}.", _timer.Period, lastEventTimeUtc);

        // If cancellation was requested, skip clock time updates
        if (!_cts.IsCancellationRequested)
        {
            await _timer.WaitForNextTickAsync(_cts.Token);
            
            OnTimerCompleted(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Toggles the paused state of the timer.
    /// </summary>
    public void TogglePause()
    {
        if (IsRunning && RemainingSeconds > 0)
        {
            double elapsedTime = DateTimeOffset.UtcNow.Subtract(lastEventTimeUtc).TotalSeconds;

            // for differences less than a second, clamp the result to 0 seconds remaining
            RemainingSeconds = elapsedTime >= 1 ? RemainingSeconds - elapsedTime : 0;
        }

        IsRunning = !IsRunning;
        lastEventTimeUtc = DateTimeOffset.UtcNow;

        logger.LogInformation("{logPrefix} timer with {RemainingSeconds} seconds left. Last Event Time (UTC): [{lastEventTimeUtc}].",
            IsRunning ? "Resuming" : "Pausing", RemainingSeconds, lastEventTimeUtc);
    }
}
