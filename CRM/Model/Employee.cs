namespace CRM.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
    }
}
