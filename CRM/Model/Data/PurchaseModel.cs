using CRM.Model.DTO;

namespace CRM.Model.Data
{
    public class PurchaseModel
    {
        public ItemDto? Item { get; set; }
        public EmployeeDto? Employee { get; set; }
        public CompanyDto? Company { get; set; }
    }
}
