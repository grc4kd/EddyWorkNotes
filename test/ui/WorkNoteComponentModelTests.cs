using System.ComponentModel.DataAnnotations;
using ui.Components.Models;

namespace test.ui;

public class WorkNoteModelTests
{
    [Fact]
    public void WorkNote_Constructor_InitializesDefaultFields()
    {
        // Arrange
        var workNote = new WorkNote();

        // Act & Assert
        Assert.Empty(workNote.Description);
    }

    [Fact]
    public void WorkNote_Should_Implement_IWorkNotes()
    {
        var workNote = new WorkNote("Test description");

        Assert.IsAssignableFrom<IWorkNote>(workNote);
    }

    [Fact]
    public void WorkNote_Constructor_Should_Set_Description()
    {
        // Arrange
        var description = "Test description";

        // Act
        var workNote = new WorkNote(description);

        // Assert
        Assert.Equal(description, workNote.Description);
    }

    [Fact]
    public void WorkNote_Constructor_Should_Truncate_Long_Description()
    {
        var longDescription = new string('A', 1001); // 1001 characters

        var workNote = new WorkNote(longDescription);

        Assert.Equal(1000, workNote.Description.Length);
        Assert.Equal(new string('A', 1000), workNote.Description);
    }

    [Fact]
    public void WorkNote_ConstructorWithEmptyDescription_ShouldSetEmptyDescription()
    {
        var workNote = new WorkNote(string.Empty);

        Assert.Empty(workNote.Description);
    }

    [Fact]
    public void WorkNote_ConstructorWithNoDescription_ShouldSetEmptyDescription()
    {
        var workNote = new WorkNote();

        Assert.Empty(workNote.Description);
    }

    [Fact]
    public void WorkNote_Description_Should_Have_StringLength_Validation()
    {
        var workNote = new WorkNote();

        var validationContext = new ValidationContext(workNote);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

        Assert.True(isValid);
    }

    [Fact]
    public void WorkNote_Description_Should_Validate_Max_Length()
    {
        var workNote = new WorkNote(new string('A', 1000)); // Exactly 1000 characters

        var validationContext = new ValidationContext(workNote);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

        Assert.True(isValid);
    }

    [Fact]
    public void WorkNote_Description_TruncatedOnConstruction()
    {
        var workNote = new WorkNote(new string('A', WorkNote.MaxWorkNoteDescription + 1));

        var validationContext = new ValidationContext(workNote);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

        Assert.True(isValid);
        Assert.Equal(1000, WorkNote.MaxWorkNoteDescription);
    }
}
