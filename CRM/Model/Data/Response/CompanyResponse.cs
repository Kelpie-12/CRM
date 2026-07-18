using CRM.Model.DTO;

namespace CRM.Model.Data.Response
{
    public class CompanyResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
        public CompanyDto? Company { get; set; }
    }
}
