using System.ComponentModel.DataAnnotations;

namespace ui.Components.Models;

public record Contact
{
    private const string ValidationErrorMessage = $"{nameof(Id)} cannot be less than 0.";

    public Contact(int id, string name, string email, string phone)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(id, 0, ValidationErrorMessage);

        Id = id;
        Name = name;
        Email = email ?? string.Empty;
        Phone = phone;
    }

    [Range(0, int.MaxValue, ErrorMessage = ValidationErrorMessage)]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}