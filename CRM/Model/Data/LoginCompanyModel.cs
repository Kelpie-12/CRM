namespace CRM.Model.Data
{
    public class LoginCompanyModel
    {
        public Company Company { get; set; }
        public User User { get; set; }
        public string Token { get; set; }

    }
}
