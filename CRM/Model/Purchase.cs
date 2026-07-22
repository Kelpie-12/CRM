namespace CRM.Model
{
    public class Purchase
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int ItemId { get; set; }
        public int ItemArticle { get; set; }
        public int Count { get; set; }
        public int FiscalDocumentNumber { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}
