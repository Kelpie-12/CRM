using CRM.Model.DTO;

namespace CRM.Model.Data
{
    public class UpdateCompanySettingsModel
    {
        public CompanyDto Company { get; set; }
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public int? CashboxId { get; set; }
    }
}
