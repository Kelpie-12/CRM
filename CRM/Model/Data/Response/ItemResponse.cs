namespace CRM.Model.Data.Response
{
    public class ItemResponse
    {
        public string ?Error{ get; set; }
        public string ?Message{ get; set; }
        public Item? Item{ get; set; }
    }
}
