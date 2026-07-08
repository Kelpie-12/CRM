using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class UserDto
    {
        public UserDto()
        {
            
        }
        [JsonProperty("id")]
        public int? Id { get; set; }
        [JsonProperty("firstname")]
        public string? FirstName { get; set; }
        [JsonProperty("lastname")]
        public string? LastName { get; set; }
        [JsonProperty("username")]
        public string? UserName { get; set; }
        [JsonProperty("password")]
        public string? Password { get; set; }    
    }
}
