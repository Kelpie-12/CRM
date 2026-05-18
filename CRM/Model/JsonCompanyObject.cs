using Newtonsoft.Json;

namespace CRM.Model
{
    public class JsonCompanyObject
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("company")]
        public List<Company> Companies { get; set; }
        [JsonProperty("jwt")]
        public string JwtToken { get; set; }
    }
}
