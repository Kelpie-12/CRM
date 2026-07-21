namespace CRM.Model.DTO
{
    public class CompanySettingsDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public int CashboxId { get; set; }
        public string CashboxName { get; set; }
        public ICollection<CashboxDto> Cashboxes { get; set; } = new List<CashboxDto>();
    }
}
