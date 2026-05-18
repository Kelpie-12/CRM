using CRM.Model;

namespace CRM.Services
{
    public interface ICompanyServices
    {
        Task<Company> GetById(int id, int userId);
        Task<string> GetAllByUser(User user);
        Task<string> CreateCompany(Company company,User user);
        Task<string> UpdateCompany(Company company);
        Task<string> DeleteCompany(int id);
        Task<string> SetRole(User user);
    }
}
