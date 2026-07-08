using CRM.Model.DTO;
using Newtonsoft.Json;

namespace CRM.Model.Data
{
    public class ClientModel
    {
        [JsonProperty("company")]
        public CompanyDto Company { get; set; }
        [JsonProperty("user")]
        public UserDto User { get; set; }
        [JsonProperty("client")]
        public ClientDto Client { get; set; }
    }
}
