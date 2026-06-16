using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class EmployeeDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
