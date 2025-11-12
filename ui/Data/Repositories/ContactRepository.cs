using DataEntities;
using DataEntities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ui.Data.Repositories;

public class ContactRepository(EddyWorkNotesContext context) : IContactRepository
{
    public async Task<List<Contact>> GetAllContacts() => await context.Contacts.ToListAsync();
    public async Task<Contact> GetContactById(int id)
    {
        var entity = await context.Contacts.FindAsync(id);

        // if the contact cannot be found, return an empty record
        if (entity == null)
        {
            return new Contact(0, string.Empty);
        }

        return entity;
    }

    public async Task AddContact(Contact contact) => await context.Contacts.AddAsync(contact);
}