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
"in the zone". This definition draws from Csíkszentmihályi, the professor who
coined the term `flow state`. I don't want to dig into the cognative science
behind attention unless it relates to a more specific application feature.

#### Algorithm: The Flow State Engine

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

## Diagrams
  {{#include ./diagrams.md}}
