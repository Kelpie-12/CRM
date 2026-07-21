namespace CRM.Model.DTO
{
    public class FiscalDocumentDto
    {
        public int Id { get; set; }
        public int? ItemId { get; set; }
        public int? Count { get; set; }
        public decimal? Amount { get; set; }
        public string? OperationType { get; set; }
        public string? StatusName { get; set; }
        public string? ProductName { get; set; }
        public int? ProductArticle { get; set; }
        public string? CashboxName { get; set; }
        public decimal? TransactionAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
