using CRM.Model.DTO;

namespace CRM.Model.Data.Response
{
    public class SaleResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
        public Sales? Sales{ get; set; }
    }
}
