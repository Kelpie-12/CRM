namespace CRM.Model.Data.Response
{
    public class CreateItemResponse
    {
        public string ?Error{ get; set; }
        public string ?Message{ get; set; }
        public Item? Item{ get; set; }
    }
}
