using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CRM.Model
{
    public class Company
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Id")]
        public int Id { get; set; }
        [JsonProperty("Status")]
        public int Status { get; set; }
        [JsonProperty("CreatedAt")]
        public DateTime CreatedAt { get; set; }

    }
}
