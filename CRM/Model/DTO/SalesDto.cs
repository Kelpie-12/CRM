using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class SalesDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("employeeId")]
        public int EmployeeId { get; set; }
        [JsonProperty("createdAt")]
        public DateTime SalesDate { get; set; }
        [JsonProperty("status")]
        public int SalesStatus { get; set; }
        [JsonProperty("count")]
        public decimal Count { get; set; }
        [JsonProperty("itemId")]
        public int ItemId { get; set; }
    }
}
