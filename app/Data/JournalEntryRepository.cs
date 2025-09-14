using Microsoft.Data.Sqlite;

namespace Eddy.Data;

public class JournalEntryRepository
{
    private readonly SqliteConnection _connection;

    public JournalEntryRepository(string db_name)
    {
        _connection = new SqliteConnection($"Data Source={db_name}");
        _connection.Open();
    }

    public void SaveJournalEntry(MoodEntry entry)
    {
        using var command = _connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO journal_entries
            (entry_class, entry_date, feedback_requested, mood_score, skill_assessment)
            VALUES (@entryClass, @entryDate, @feedbackRequested, @moodScore, @skillAssessment)";

        command.Parameters.AddWithValue("@entryClass", "MoodEntry");
        command.Parameters.AddWithValue("@entryDate", entry.EntryDate.ToString("o"));
        command.Parameters.AddWithValue("@feedbackRequested", entry.FeedbackRequested);
        command.Parameters.AddWithValue("@moodScore", entry.MoodScore);
        command.Parameters.AddWithValue("@skillAssessment", entry.SkillAssessment);
        
        command.ExecuteNonQuery();
    }
}