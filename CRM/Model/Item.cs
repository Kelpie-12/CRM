namespace CRM.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Price { get; set; }
        public int Article { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }


    }
}
