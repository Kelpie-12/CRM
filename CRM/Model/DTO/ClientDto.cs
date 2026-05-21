using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class ClientDto
    {   
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        public string LastName { get; set; }     
    }
}
