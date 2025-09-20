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
    private PeriodicTimer _timer = new(TimeSpan.FromMilliseconds(1));

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
    public double RemainingSeconds { get; set; }

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
    /// Resets the timer to its initial state based on current work/break period.
    /// </summary>
    private void ResetTimer()
    {
        lastEventTimeUtc = DateTimeOffset.UtcNow;
        logger.LogInformation("Starting timer with {RemainingTime} work minutes at {lastEvenTimeUtc}.", RemainingSeconds, lastEventTimeUtc);
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

        RemainingSeconds = _timer.Period.TotalSeconds;
        IsRunning = true;
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
        ResetTimer();

        // If cancellation was requested, skip clock time updates
        if (!_cts.IsCancellationRequested)
        {
            await UpdateTimeAsync(_cts.Token);
        }
    }

    /// <summary>
    /// Toggles the paused state of the timer.
    /// </summary>
    public void TogglePause()
    {
        if (IsRunning)
        {
            double elapsedTime = DateTimeOffset.UtcNow.Subtract(lastEventTimeUtc).TotalSeconds;
            RemainingSeconds -= elapsedTime;
        }

        IsRunning = !IsRunning;
        lastEventTimeUtc = DateTimeOffset.UtcNow;

        logger.LogInformation("{logPrefix} timer with {RemainingSeconds} seconds left. Last Event Time (UTC): [{lastEventTimeUtc}].",
            IsRunning ? "Resuming" : "Pausing", RemainingSeconds, lastEventTimeUtc);

        _timer.Dispose();
        OnTimerCompleted(EventArgs.Empty);
    }

    /// <summary>
    /// Updates the timer's remaining time until cancellation or completion.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
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
}
