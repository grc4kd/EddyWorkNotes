namespace Eddy
{
    public record MoodEntry
    {
        private readonly UserFeedbackLoop _feedbackLoop = new();

        public DateTime EntryDate { get; set; }
        public double MoodScore { get; set; }

        // Skill assessment properties
        public Dictionary<string, double> SkillAssessment { get; set; } = [];

        public bool FeedbackRequested { get; set; }

        public void RecordSkillAssessment(string skill, double rating)
        {
            _feedbackLoop.CollectSelfAssessment(skill, rating);

            if (!SkillAssessment.TryAdd(skill, rating))
                throw new ArgumentException(
                    $"Unable to add rating '{rating}' for skill '{skill}'"
                );
        }

        public void RequestFeedbackForSkills(IEnumerable<string> skills)
        {
            foreach (var skill in skills)
            {
                if (!SkillAssessment.TryGetValue(skill, out double value))
                    SkillAssessment.Add(skill, 0);

                _feedbackLoop.SetUserGoal(skill, value * 1.2);
            }

            FeedbackRequested = true;
        }
    }
}
