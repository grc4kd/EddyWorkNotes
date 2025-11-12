using System.ComponentModel.DataAnnotations;

namespace ui.Components.Models;

public record WorkNote
{
    public const int MaxWorkNoteDescription = 1000;
    // must be constant for StringLength validator attribute
    private const string ValidationErrorMessage = $"Work notes description is too long (1000 character limit).";

    public WorkNote(string description = "")
    {
        // truncate description input to maximum length in data layer
        int strlen = description.Length <= MaxWorkNoteDescription ? description.Length : MaxWorkNoteDescription;
        Description = description[..strlen];
    }

    [StringLength(MaxWorkNoteDescription, ErrorMessage = ValidationErrorMessage)]
    public string Description { get; set; }
}