using Newtonsoft.Json;

namespace CRM.Model
{
    public class Client
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("FirstName")]
        public string FirstName { get; set; }
        [JsonProperty("LastName")]
        public string LastName { get; set; }     
   

        [JsonProperty("Status")]
        public string Status { get; set; }

    }
}
