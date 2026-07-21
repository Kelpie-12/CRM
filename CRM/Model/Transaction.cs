namespace CRM.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public string CashboxName { get; set; } = string.Empty;
        public string OperationType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int FiscalDocumentNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }
}
