namespace Eddy;

public class TaskTimer
{
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource _cts = new();

    public int WorkDuration { get; set; } = 25 * 60; // Default 25 minutes in seconds
    public int BreakDuration { get; set; } = 5 * 60;   // Default 5 minutes in seconds
    public bool IsRunning { get; private set; }
    public bool IsWorkTime { get; private set; }
    public double RemainingTime { get; private set; }

    public event EventHandler? TimeElapsed;
    public event EventHandler? TimerCompleted;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startWithWork"></param>
    public TaskTimer(bool startWithWork = true)
    {
        _timer = new(TimeSpan.FromSeconds(startWithWork switch
        {
            true => WorkDuration,
            false => BreakDuration
        }));

        IsWorkTime = startWithWork;
        // until Start method is called, do not set running timer state
        IsRunning = false;
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
        RemainingTime = IsWorkTime ? WorkDuration : BreakDuration;

        if (_cts != null)
            TogglePause();

        _cts = new CancellationTokenSource();
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

        await _timer.WaitForNextTickAsync(_cts.Token);
    
        // Auto-switch between work and break
        IsWorkTime = !IsWorkTime;
        await StartAsync();
        
        RemainingTime = 0;
        IsRunning = false;
    }
}