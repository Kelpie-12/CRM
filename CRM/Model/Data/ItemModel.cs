using CRM.Model.DTO;

namespace CRM.Model.Data
{
    public class ItemModel
    {
        public UserDto User {  get; set; }  
        public CompanyDto Company { get; set; } 
        public Item Item { get; set; }  
    }
}
