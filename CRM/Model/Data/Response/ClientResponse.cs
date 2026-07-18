using CRM.Model.DTO;

namespace CRM.Model.Data.Response
{
    public class ClientResponse
    {
        public string? Message { get; set; }
        public string? Error { get; set; }
        public Client? Client { get; set; }
    }
}
