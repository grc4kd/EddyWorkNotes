using Eddy;

namespace test;

public class MoodEntryTest
{
    [Fact]
    public void CreateMoodEntry_WithValidValues_ShouldInitializeCorrectly()
    {
        // Arrange
        var entry = new MoodEntry
        {
            EntryDate = DateTime.Parse("2025-01-01"),
            MoodScore = 0.8,
            SkillAssessment = new Dictionary<string, double>
            {
                ["Coding"] = 4.5,
                ["Testing"] = 3.8
            },
            FeedbackRequested = true
        };

        // Assert
        Assert.Equal(DateTime.Parse("2025-01-01"), entry.EntryDate);
        Assert.Equal(0.8, entry.MoodScore);
        Assert.Equal(2, entry.SkillAssessment.Count);
        Assert.Equal(4.5, entry.SkillAssessment["Coding"]);
        Assert.Equal(3.8, entry.SkillAssessment["Testing"]);
        Assert.True(entry.FeedbackRequested);
    }

    [Fact]
    public void EntryDate_SetToFutureDate_ShouldBeAccepted()
    {
        // Arrange
        var entry = new MoodEntry();
        var futureDate = DateTime.Now.AddDays(1);

        // Act
        entry.EntryDate = futureDate;

        // Assert
        Assert.Equal(futureDate, entry.EntryDate);
    }

    [Fact]
    public void MoodScore_SetToValidRange_ShouldBeAccepted()
    {
        // Arrange
        var entry = new MoodEntry
        {
            // Act
            MoodScore = 0.5
        };

        // Assert
        Assert.Equal(0.5, entry.MoodScore);
    }

    [Fact]
    public void MoodScore_SetToInvalidRange_ShouldBeAccepted()
    {
        // Arrange
        var entry = new MoodEntry
        {
            // Act
            MoodScore = -0.1
        };

        // Assert
        Assert.Equal(-0.1, entry.MoodScore);
    }

    [Fact]
    public void SkillAssessment_AddSkill_ShouldBeAddedToDictionary()
    {
        // Arrange
        var entry = new MoodEntry();

        // Act
        entry.SkillAssessment.Add("Communication", 4.0);

        // Assert
        Assert.Single(entry.SkillAssessment);
        Assert.Equal(4.0, entry.SkillAssessment["Communication"]);
    }

    [Fact]
    public void SkillAssessment_AddMultipleSkills_ShouldBeAddedToDictionary()
    {
        // Arrange
        var entry = new MoodEntry();

        // Act
        entry.SkillAssessment.Add("Coding", 4.5);
        entry.SkillAssessment.Add("Testing", 3.8);
        entry.SkillAssessment.Add("Communication", 4.0);

        // Assert
        Assert.Equal(3, entry.SkillAssessment.Count);
        Assert.Equal(4.5, entry.SkillAssessment["Coding"]);
        Assert.Equal(3.8, entry.SkillAssessment["Testing"]);
        Assert.Equal(4.0, entry.SkillAssessment["Communication"]);
    }

    [Fact]
    public void FeedbackRequested_SetToTrue_ShouldBeTrue()
    {
        // Arrange
        var entry = new MoodEntry
        {
            // Act
            FeedbackRequested = true
        };

        // Assert
        Assert.True(entry.FeedbackRequested);
    }

    [Fact]
    public void FeedbackRequested_SetToFalse_ShouldBeFalse()
    {
        // Arrange
        var entry = new MoodEntry
        {
            // Act
            FeedbackRequested = false
        };

        // Assert
        Assert.False(entry.FeedbackRequested);
    }

    [Fact]
    public void RecordSkillAssessment_ValidSkillAndRating_ShouldUpdateSkillAssessment()
    {
        // Arrange
        var entry = new MoodEntry();

        // Act
        entry.RecordSkillAssessment("Problem Solving", 4.2);

        // Assert
        Assert.Single(entry.SkillAssessment);
        Assert.Equal(4.2, entry.SkillAssessment["Problem Solving"]);
    }

    [Fact]
    public void RequestFeedbackForSkills_ValidSkills_ShouldSetFeedbackRequestedToTrue()
    {
        // Arrange
        var entry = new MoodEntry();
        entry.SkillAssessment.Add("Coding", 4.5);
        entry.SkillAssessment.Add("Testing", 3.8);

        // Act
        entry.RequestFeedbackForSkills(["Coding", "Testing"]);

        // Assert
        Assert.True(entry.FeedbackRequested);
    }
}
