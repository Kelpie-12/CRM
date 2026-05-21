using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Repositories;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CRM.Services.Implementations
{
    public class CompanyServices : ICompanyServices
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtServices _jwtServices;
        private JsonSerializerOptions _options;
        public CompanyServices(ICompanyRepository companyRepository, IJwtServices jwtServices, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _jwtServices = jwtServices;
            _companyRepository = companyRepository;
            _options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                WriteIndented = true
            };
        }
        public Task<string> CreateCompany(CompanyDto company, UserDto user)
        {
            //создавать одельную базу данных с базовами таблицами:
            //1. Задачи +
            //2. Продажи +
            //3. Клиенты +
            //4. Настройки 
            //5. Склад +
            //6. Работники +


            return _companyRepository.CreateCompany(company, user);
        }

        public Task<string> DeleteCompany(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAllByUser(User user)
        {
            List<Company> companies = await _companyRepository.GetAllByUser(user);

            JsonCompaniesListModel json = new JsonCompaniesListModel()
            {
                UserId = user.Id,
                Companies = companies,
                JwtToken = _jwtServices.GenerateToken(user)
            };
            string companyJson = JsonSerializer.Serialize(json, _options);

            return companyJson;
        }

        public async Task<LoginCompanyModel> GetById(int id, int userId)
        {
            //возможны проверки
            LoginCompanyModel model = new LoginCompanyModel();
            model.Company = await _companyRepository.GetById(id, userId);
            model.User = await _userRepository.GetUser(userId, id);
            model.JWT = _jwtServices.GenerateToken(model.User);
            return model;
            //throw new NotImplementedException();
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
