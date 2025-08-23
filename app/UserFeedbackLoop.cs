using System.Collections;
using System.Data;
using Microsoft.VisualBasic;

namespace Eddy;

public class UserFeedbackLoop
{
    private readonly Dictionary<string, double> _userGoals = [];
    private Dictionary<string, double> _currentMetrics = [];

    private readonly IList<FeedbackRecord> feedbackRecords = [];

    public void CollectSelfAssessment(string skill, double userRating)
    {
        // if no current metric value is found, start at the userRating
        if (!_currentMetrics.TryGetValue(skill, out double value))
            value = userRating;

        // Store the self-assessment for later analysis
        feedbackRecords.Add(
            new FeedbackRecord()
            {
                Skill = skill,
                Rating = userRating,
                CurrentMetricValue = value,
                Timestamp = DateTime.Now
            }
        );


    }

    public void SetUserGoal(string skill, double targetLevel)
    {
        if (!_userGoals.TryAdd(skill, targetLevel))
            throw new ArgumentException($"Error adding skill ;{skill}' with target level: {targetLevel}.");
    }

    public Dictionary<string, double> GetAdjustedMetrics()
    {
        var adjustedMetrics = new Dictionary<string, double>(_currentMetrics);

        // Apply goal-based adjustments
        foreach (var skill in _userGoals.Keys)
        {
            if (_currentMetrics.TryGetValue(skill, out var currentValue))
            {
                var targetValue = _userGoals[skill];
                var adjustmentFactor = Math.Abs(targetValue - currentValue) /
                    Math.Max(targetValue, currentValue);

                adjustedMetrics[skill] += (targetValue > currentValue) ?
                    adjustmentFactor * 0.2 :
                    -adjustmentFactor * 0.1;
            }
        }

        return adjustedMetrics;
    }

    public List<string> GetImprovementRecommendations()
    {
        var recommendations = new List<string>();

        foreach (var skill in _currentMetrics.Keys)
        {
            if (!_userGoals.ContainsKey(skill)) continue;

            var currentValue = _currentMetrics[skill];
            var targetValue = _userGoals[skill];

            if (currentValue < targetValue * 0.8)
                recommendations.Add(
                    $"Work on improving {skill} - you're below your target goal");

            else if (currentValue < targetValue)
                recommendations.Add(
                    $"Keep working on {skill} to reach your target goal");
        }

        return recommendations;
    }

    public void UpdateCurrentMetrics(Dictionary<string, double> newMetrics)
    {
        _currentMetrics = newMetrics;
    }
}

public class FeedbackRecord
{
    public required DateTime Timestamp { get; set; }
    public required string Skill { get; set; }
    public double Rating { get; set; }
    public double CurrentMetricValue { get; set; }
    public string? Comments { get; set; }
}
