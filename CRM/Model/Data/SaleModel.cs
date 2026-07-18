using System.Data.SqlTypes;
using CRM.Model.DTO;
using Newtonsoft.Json;

namespace CRM.Model.Data
{
    public class SaleModel
    {    
       
        public ItemDto? Item{ get; set; }       
        public EmployeeDto? Employee{ get; set; }     
        public ClientDto? Client { get; set; }
        public CompanyDto ?Company{ get; set; } 
    }
}
