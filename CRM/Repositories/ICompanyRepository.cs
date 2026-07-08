using CRM.Model;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> GetById(int id,int userId);
        Task<List<Company>> GetAllByUser(User user);
        Task<CreateCompanyResponse> CreateCompany(CompanyDto company, UserDto user);
        Task<string> UpdateCompany(Company company);
        Task<string> DeleteCompany(int id);
        Task<string> SetRole(User user);
      

    }
}
