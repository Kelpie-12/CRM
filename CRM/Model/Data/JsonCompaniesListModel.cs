using Newtonsoft.Json;

namespace CRM.Model.Data
{
    /*
         {
          "token": "jwt...",
          "user": { "username": "john" },
          "companies": [
            { "id": "1", "name": "Acme Corp" },
            { "id": "2", "name": "Globex" }

        }*/
    public class JsonCompaniesListModel    
    {
      
        public User User { get; set; }        
        public List<Company> Companies { get; set; }
        public string Token { get; set; }
    }
}


/*
 The main panel should look like this:
1. On the left is a panel with buttons: product, clients, analytics, settings. At the bottom is the user profile with name and job title.
2. In the upper right corner is the search bar.
3. In the center is information about the selected page; if it's about clients, it's the client information.*/