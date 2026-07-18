using Newtonsoft.Json;

namespace CRM.Model
{
    public class Sales
    {
        public int EmployeeId { get; set; }      
        public int Id { get; set; }   
        public DateTime SalesDate { get; set; }       
        public int Status { get; set; }       
        public decimal Count{ get; set; }      
        public int ItemId { get; set; } 
        public int Article {  get; set; }  
        public int? TransitionId { get; set; }

    }
}
