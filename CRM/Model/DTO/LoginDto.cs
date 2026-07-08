using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class LoginDto
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
