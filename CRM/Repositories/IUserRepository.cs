namespace CRM.Repositories
{
    public interface IUserRepository
    {
        Task<string> Login(string username, string password);
        Task<string> CreateUser(string username, string password, string firstName, string lastName);

    }
}
