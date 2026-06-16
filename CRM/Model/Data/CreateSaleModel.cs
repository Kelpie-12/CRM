using System.Data.SqlTypes;
using CRM.Model.DTO;
using Newtonsoft.Json;

namespace CRM.Model.Data
{
    public class CreateSaleModel
    {    
        [JsonProperty("item")]
        public ItemDto Item{ get; set; }
        [JsonProperty("employee")]
        public EmployeeDto Employee{ get; set; }
        [JsonProperty("company")]
        public CompanyDto Company{ get; set; } 
    }
}
