using Newtonsoft.Json;

namespace CRM.Model
{
    public class Sales
    {
        [JsonProperty("")]
        public int IdSalesman { get; set; }
        public int SalesId { get; set; }
        public DateTime SalesDate { get; set; }
        public int SalesStatus { get; set; }
        public decimal SalesTotal { get; set; }
        public int ItemId { get; set; } 

    }
}
