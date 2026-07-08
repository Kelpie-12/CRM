using Newtonsoft.Json;

namespace CRM.Model.DTO
{
    public class LoginCompanyDto
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("companyId")]
        public int CompanyId { get; set; }
    }
}
