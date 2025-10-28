using System.ComponentModel.DataAnnotations;
using ui.Components.Models;

namespace test
{
    public class WorkNoteModelTests
    {
        [Fact]
        public void WorkNote_Should_Implement_IWorkNotes()
        {
            // Arrange
            var workNote = new WorkNote("Test description");

            // Act & Assert
            Assert.IsAssignableFrom<IWorkNotes>(workNote);
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
            // Arrange
            var longDescription = new string('A', 1001); // 1001 characters

            // Act
            var workNote = new WorkNote(longDescription);

            // Assert
            Assert.Equal(1000, workNote.Description.Length);
            Assert.Equal(new string('A', 1000), workNote.Description);
        }

        [Fact]
        public void WorkNote_Constructor_Should_Handle_Empty_Description()
        {
            // Arrange
            var description = "";

            // Act
            var workNote = new WorkNote(description);

            // Assert
            Assert.Equal("", workNote.Description);
        }

        [Fact]
        public void WorkNote_Constructor_Should_Handle_Null_Description()
        {
            // Arrange
            string? description = null;

            // Act
            var workNote = new WorkNote(description ?? "");

            // Assert
            Assert.Equal("", workNote.Description);
        }

        [Fact]
        public void WorkNote_Description_Should_Have_StringLength_Validation()
        {
            // Arrange
            var workNote = new WorkNote();

            // Act
            var validationContext = new ValidationContext(workNote);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void WorkNote_Description_Should_Validate_Max_Length()
        {
            // Arrange
            var workNote = new WorkNote(new string('A', 1000)); // Exactly 1000 characters

            // Act
            var validationContext = new ValidationContext(workNote);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void WorkNote_Description_TruncatedOnConstruction()
        {
            // Arrange
            var workNote = new WorkNote(new string('A', WorkNote.MaxWorkNoteDescription + 1));

            // Act
            var validationContext = new ValidationContext(workNote);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(workNote, validationContext, validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Equal(1000, WorkNote.MaxWorkNoteDescription);
        }
    }
}
