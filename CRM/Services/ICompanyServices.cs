using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
using CRM.Model.DTO;

namespace CRM.Services
{
    public interface ICompanyServices
    {
        Task<LoginCompanyModel> GetById(int id, int userId);
        Task<LoginCompanyModel> GetLoginCompany(int id, int userId);
        Task<string> GetAllByUser(User user);
        Task<CompanyResponse> CreateCompany(CompanyDto company,UserDto user);
        Task<string> UpdateCompany(Company company);
        Task<string> DeleteCompany(int id);
        Task<string> SetRole(User user);
    }
}
