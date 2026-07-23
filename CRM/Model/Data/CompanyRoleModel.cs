using CRM.Model.DTO;

namespace CRM.Model.Data
{
    public class CompanyRoleModel
    {
        public CompanyDto Company { get; set; } = new CompanyDto();
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
