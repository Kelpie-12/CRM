using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class ItemDto
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public decimal? PurchasePrice { get; set; }
    }
}
