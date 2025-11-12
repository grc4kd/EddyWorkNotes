using System.ComponentModel.DataAnnotations;
using ui.Components.Models;

namespace test.ui;

public class ContactTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        int id = 1;
        string name = "John Doe";
        string email = "john@example.com";
        string phone = "123-456-7890";

        var contact = new Contact(id, name, email, phone);

        Assert.Equal(id, contact.Id);
        Assert.Equal(name, contact.Name);
        Assert.Equal(email, contact.Email);
        Assert.Equal(phone, contact.Phone);
    }

    [Fact]
    public void Id_SetToNegativeValue_ShouldThrowValidationException()
    {
        const int invalidId = -1;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            var contact = new Contact(invalidId, "John Doe", "john@example.com", "123-456-1234");
        });
    }

    [Fact]
    public void Id_SetToMaxValue_ShouldNotThrowException()
    {
        var contact = new Contact(0, "John Doe", "john@example.com", "123-456-1234");
        int maxValue = int.MaxValue;

        contact.Id = maxValue;

        Assert.Equal(maxValue, contact.Id);
    }

    [Fact]
    public void Name_SetToEmptyString_ShouldBeEmpty()
    {
        var contact = new Contact(1, "", "john@example.com", "123-456-1234");

        string name = contact.Name;

        Assert.Equal(string.Empty, name);
    }

    [Fact]
    public void Email_SetToNull_ShouldReplaceWithEmptyString()
    {
        var contact = new Contact(1, "John Doe", null!, "123-456-1234");

        string email = contact.Email!;

        Assert.Empty(email);
    }

    [Fact]
    public void Phone_SetToEmptyString_ShouldBeEmpty()
    {
        var contact = new Contact(1, "John Doe", "john@example.com", "");

        string phone = contact.Phone!;

        Assert.Equal(string.Empty, phone);
    }
}