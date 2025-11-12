namespace DataEntities.Interfaces
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllContacts();
        Task<Contact> GetContactById(int id);
        Task AddContact(Contact contact);
    }
}