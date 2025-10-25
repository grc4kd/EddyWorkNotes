# Eddy - A Project Management Personal (Virtual) Assistant

Eddy is a project management tool catered to individuals with durable memory
that want to enhance their relative shortage in working, short-term memory.
There are many reasons why one might need more working memory than they had
been granted prior, but there are many tools and techniques to accelerate
research and development without breaking flow state through automation.

## Definitions

### Flow state

For the purposes of this application, flow state shall be defined to be a
mental state in which a person is fully immersed in a task, requiring little
attention, to the point where it seems effortless. It is also called being
"in the zone". This definition draws from Mihaly Robert Csikszentmihalyi, the 
professor who coined the term `flow state`. I don't want to dig into the 
cognitive science behind attention unless it relates to a more specific 
application feature.

## Usage Instructions

Eddy - Example Usage Instructions

These instructions outline how to use Eddy during your typical workflow. Assume
you are using Eddy to track your progress on a coding project.

1. Starting a New Task/Session:

    - Record Initial Mood: Before you begin coding, open Eddy and record your
      current mood score (e.g., 0.6 for slightly unmotivated). This provides a
      baseline.
    - Define Skill Focus: Identify the primary skill you'll be working on (e.g.,
      "Python Debugging").
    - Set Initial Goal: Set a target level for that skill (e.g., 0.8).  Eddy will
      track your progress towards this.

2. During Coding:

    - Skill Assessment: As you encounter challenges, periodically (e.g., every
      hour or after completing a significant feature) record a skill assessment
      for the skill you're focusing on. Be honest!  A score of 0.7 might indicate
      you're making progress but still encountering difficulties.
    - Mood Tracking: Briefly update your mood score if it changes significantly.
      A frustrating bug might lower it, while a successful implementation could
      raise it.

3. Requesting Feedback:

    - Trigger Feedback Request: After completing a module or feature, use Eddy to
      request feedback on the skills you've been working on (e.g., "Python
      Debugging", "Code Readability").
    - Specify Feedback Areas: If you have specific areas you'd like feedback on
      (e.g., "Error handling in the authentication module"), include that in your
      request.

4. Receiving and Processing Feedback:

    - Record Feedback: When you receive feedback (from a peer review, mentor, or
      self-assessment), record it in Eddy. Include the rating and comments.
    - Update Current Metrics: Eddy will automatically update your current skill
      metrics based on the feedback.
    - Review Adjusted Metrics: Examine the adjusted metrics to identify areas for
      improvement.

5. Iterating and Improving:

    - Review Improvement Recommendations: Eddy will provide recommendations based
      on your metrics (e.g., "Focus on unit testing to improve code
      reliability").
    - Set New Goals: Adjust your skill goals based on the recommendations and
      your progress.
    - Repeat: Continue this cycle of assessment, feedback, and improvement to
      enhance your skills and maintain a positive flow state.

- Example Scenario:

1. Start
    - Mood: 0.6, Skill: "Python Debugging", Goal: 0.8
2. Coding (2 hours)
    - Skill Assessment: 0.7 (encountering some tricky bugs)
3. Request Feedback
    - Request feedback on "Python Debugging" - specifically error handling.
4. Receive Feedback
    - Peer review rates error handling as 3/5 with comments: "Good overall, but could
      use more descriptive error messages."
5. Eddy Updates
    - Skill metric for "Python Debugging" updated to 0.75.  Recommendation: "Improve
      error message clarity."
6. Next Session
    - Focus on writing more descriptive error messages.

These instructions are a starting point. As you become more familiar with Eddy,
you can customize your workflow to suit your individual needs and preferences.

## Algorithms

### Flow State Engine

```python
def flow_state(skill_level, challenge_level, feedback_quality):
  """
  Simulates the process leading to a potential flow state.

  Args:
    skill_level:  A numerical representation of the person's skill. 
    (e.g., 1-10)
    challenge_level: A numerical representation of the activity's difficulty. 
    (e.g., 1-10)
    feedback_quality: A rating of the clarity and immediacy of feedback. 
    (e.g., 0-1, higher is better)

  Returns:
    A tuple: (flow_potential, emotional_state)
    flow_potential: A float representing the likelihood of entering flow 
    (e.g., 0.0-1.0)
    emotional_state: A string indicating the dominant emotion.
    (e.g., "Engaged", "Boredom", "Focused", "Confident", "Elated")
  """

  difference = abs(skill_level - challenge_level)

  if difference > 3: # Significant mismatch
    if skill_level > challenge_level:
      emotional_state = "Boredom"
      flow_potential = 0.0
    else:
      emotional_state = "Anxiety"
      flow_potential = 0.0
  elif difference <= 3:
    # Flow potential increases as difference decreases
    flow_potential = 0.5 + (0.5 * (1 - (difference / 3))) 
    # Good feedback boosts flow.
    flow_potential = flow_potential * (1 + feedback_quality * 0.5)
    # Base emotional state, will be refined.
    emotional_state = "Focused"  
    
    if flow_potential > 0.8:
      emotional_state = "Engaged"
    elif flow_potential > 0.6:
      emotional_state = "Interested"

  return (flow_potential, emotional_state)


# Example Usage
skill = 7
challenge = 6
feedback = 0.8

potential, emotion = flow_state(skill, challenge, feedback)
print(f"Flow Potential: {potential:.2f}, Emotional State: {emotion}")
```

[Link to Mermaid Diagram: Decision Diagram -- Flow State Monitoring](svgs/README.md-1.svg)

#### Mermaid Diagram Code

```mermaid
graph TD
    A[Start: Skill Level & Challenge Level Input] --> B
    B{ Skill Level - Challenge Level > 3?};
    B -- Yes --> C{ Skill Level > Challenge Level? };
    C -- Yes --> D[Emotional State: Boredom, Flow Potential: 0.0];
    C -- No --> E[Emotional State: Anxiety, Flow Potential: 0.0];
    B -- No --> F[Calculate Initial Flow Potential];
    F --> G{ Feedback Quality > 0?};
    G -- Yes --> H[Adjust Flow Potential, Basis Feedback Quality];
    H --> I{Flow Potential > 0.8?};
    I -- Yes --> J[Emotional State: Engaged];
    I -- No --> K{Flow Potential > 0.6?};
    K -- Yes --> L[Emotional State: Interested];
    K -- No --> M[Emotional State: Focused];
    G -- No --> M;
    M --> N[End: Flow State Achieved, Result Potential/Emotion];
    N --> O[Optional: Continued engagement, Result Stronger Flow];
    J --> N;
    L --> N;
