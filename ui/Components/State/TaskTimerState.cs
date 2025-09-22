using System.Text;

namespace ui.Components.State;

public record TaskTimerState
{
    private static readonly Dictionary<int, string> ValidStates = [];
    private string timerStatus = ValidStates.FirstOrDefault().Value;

    /// <summary>
    /// A simple check for input strings against a dictionary of indexed values.
    /// </summary>
    /// <param name="inputStr">Case-sensitive input string to be validated.</param>
    /// <returns>true if input string is valid; false otherwise.</returns>
    public static bool ValidateString(string inputStr) => ValidStates.ContainsValue(inputStr);
    public string TimerStatus
    {
        get => timerStatus;
        init
        {
            timerStatus = ValidateString(value)
                ? ValidateString(value) ? value : ValidStates.First().Value
                : throw new ArgumentOutOfRangeException(nameof(value),
                    $"Expected {value} to be in range of expected states: [{PrintValidStates}]");
        }
    }
    static TaskTimerState()
    {
        LoadValidStates();
    }

    public TaskTimerState(string status) => TimerStatus = status;

    private static void LoadValidStates()
    {
        ValidStates.Add(0, "Stopped");
        ValidStates.Add(1, "Running");
        ValidStates.Add(2, "Paused");
        ValidStates.Add(3, "Resumed");
    }

    private static string PrintValidStates()
    {
        var sbStates = new StringBuilder();
        foreach (var state in ValidStates)
        {
            sbStates.Append(state + ", ");
        }
        return sbStates.ToString();
    }
}