using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class ItemDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }  
    }
}
