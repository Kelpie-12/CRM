using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CRM.Model;
using CRM.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CRM.Services.Implementations
{
    public class CompanyServices : ICompanyServices
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IJwtServices _jwtServices;
        private JsonSerializerOptions _options;
        public CompanyServices(ICompanyRepository companyRepository, IJwtServices jwtServices)
        {
            _jwtServices = jwtServices;
            _companyRepository = companyRepository;
            _options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
        }
        public Task<string> CreateCompany(Company company,User user)
        {
            return _companyRepository.CreateCompany(company, user);            
        }

        public Task<string> DeleteCompany(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAllByUser(User user)
        {
            List<Company> companies = await _companyRepository.GetAllByUser(user);

            JsonCompanyObject json = new JsonCompanyObject()
            {
                UserId = user.Id,
                Companies = companies,
                JwtToken = _jwtServices.GenerateToken(user)
            };
            string companyJson = JsonSerializer.Serialize(json, _options);

            return companyJson;
        }

        public Task<Company> GetById(int id, int userId)
        {
            return _companyRepository.GetById(id,userId);
            throw new NotImplementedException();
        }

        public Task<string> SetRole(User user)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateCompany(Company company)
        {
            throw new NotImplementedException();
        }
    }
}
