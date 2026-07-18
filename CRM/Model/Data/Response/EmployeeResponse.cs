using CRM.Model;

namespace CRM.Model.Data.Response
{
    public class EmployeeResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
        public Employee? Employee { get; set; }
    }
}
