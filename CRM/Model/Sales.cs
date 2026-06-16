using Newtonsoft.Json;

namespace CRM.Model
{
    public class Sales
    {
        [JsonProperty("employeeId")]
        public int EmployeeId { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("createdAt")]
        public DateTime SalesDate { get; set; }
        [JsonProperty("status")]
        public int SalesStatus { get; set; }
        [JsonProperty("count")]
        public decimal Count{ get; set; }
        [JsonProperty("itemId")]
        public int ItemId { get; set; } 

        [JsonProperty("article")]
        public int ItetmArticle {  get; set; }  

    }
}
