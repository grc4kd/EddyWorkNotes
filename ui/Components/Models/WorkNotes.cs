using System.ComponentModel.DataAnnotations;

namespace ui.Components.Models;

public class WorkNotes
{
    [StringLength(1000, ErrorMessage = "Work notes description is too long (1000 character limit).")]
    public string? Description { get; set; } = string.Empty;
}