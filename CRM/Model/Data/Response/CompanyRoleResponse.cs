using CRM.Model.DTO;

namespace CRM.Model.Data.Response
{
    public class CompanyRoleResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = "none";
        public CompanyRoleDto? Role { get; set; }
    }
}
