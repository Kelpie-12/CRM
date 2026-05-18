using CRM.Model.DTO;

namespace CRM.Model.Data
{
    public class CreateCompanyModel
    {
        public CompanyDto Company { get; set; }
        public UserDto User { get; set; }
    }
}
