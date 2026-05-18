using CRM.Model;

namespace CRM.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> GetById(int id,int userId);
        Task<List<Company>> GetAllByUser(User user);
        Task<string> CreateCompany(Company company,User user);
        Task<string> UpdateCompany(Company company);
        Task<string> DeleteCompany(int id);
        Task<string> SetRole(User user);

    }
}
