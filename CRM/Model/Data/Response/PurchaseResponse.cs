using CRM.Model;

namespace CRM.Model.Data.Response
{
    public class PurchaseResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = "none";
        public Purchase Purchase { get; set; } = new();
    }
}
