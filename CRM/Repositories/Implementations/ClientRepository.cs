using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CRM.Repositories.Implementations
{
    public class ClientRepository : BaseRepository, IClientRepository
    {
        public ClientRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public async Task<Client> CreateClient(CompanyDto company, ClientDto client, UserDto user)
        {
            Client res =  new Client(); 
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Clients].FirstName=@FirstName and [Clients].LastName=@LastName) " +
                                      "BEGIN " +
                                      "INSERT INTO [Clients] ([Clients].FirstName,[Clients].LastName,[Clients].[Status],[Clients].CreatedByUserId) " +
                                      "VALUES (@FirstName,@LastName,@Status,@UserId); " +
                                      "SELECT Clients.FirstName,Clients.Id,Clients.LastName,[Status].[Name] " +
                                      "from [Clients],[Status] " +
                                      "where Clients.Id=(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                                      "and [Status].Id=[Clients].[Status] " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Client already exist' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Active);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) != "Client already exist")
                            {                               
                                res.FirstName = reader.GetString(0);
                                res.Id = reader.GetInt32(1);
                                res.LastName = reader.GetString(2);
                                res.Status = reader.GetString(3);
                            }
                            else
                            {
                                res.Id = -1;
                            }

                        }
                    }
                }
                con.Close();
            }
            return res;
        }

        public async Task<Client> CreateSalesForClient(string data)
        {
            Sales sales = new Sales();
            Client client = new Client();

            return await Task.FromResult(client);

            throw new NotImplementedException();
        }

        public async Task<Client> UpdateClient(CompanyDto company, ClientDto client, UserDto user)

        {
            Client res = new Client();
            string companyName = company.Name;           

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "update [Clients] " +
                        "set [FirstName]=@FirstName, [LastName]=@LastName, [Status]=@Status,[UpdateByUserId]=@UserId,UpdatedAt=@date " +
                        "where [Clients].Id=@ClientId " +
                        //"(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                        "select Clients.Id,Clients.FirstName,Clients.LastName,[Status].[Name] " +
                        "from [Clients],[Status] " +
                        "where Clients.Id=@ClientId " +
                        //"(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                        "and [Status].Id=[Clients].[Status] ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("ClientId", client.Id);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    //cmd.Parameters.AddWithValue("ClientId", client.Id);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Active);
                    cmd.Parameters.AddWithValue("date", DateTime.Now);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {                            
                                res.Id = reader.GetInt32(0);
                                res.FirstName = reader.GetString(1);
                                res.LastName = reader.GetString(2);
                                res.Status = reader.GetString(3);                            
                        }
                    }
                }
                con.Close();
            }
            return res;

        }

        public Task<Client> GetInfoClient(InfoClientModel model)
        {
            //реализовать метод возварщающий клиента и его покупки/продажи

            Client res = new Client()
            {
                Id = -1,
                FirstName = "test",
                LastName = "testLast",
                Status = "Active"
            };
            return Task.FromResult(res);
        }

        public async Task<ICollection<Client>> GetInfoListClients(int companyId)
        {
            //возвращать лист клиентов
            List<Client> res = new List<Client>();
            string companyName = "";
            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select Company.[Name] from Company where Company.Id=@id";
                    cmd.Parameters.AddWithValue("id", companyId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                            companyName = reader.GetString(0);
                    }
                }
            }
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select Clients.Id,Clients.FirstName,Clients.LastName from Clients where Clients.Status=@Status";

                    cmd.Parameters.AddWithValue("Status", Status.Active);

                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            res.Add(
                                new Client()
                                {
                                    Id = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2)
                                }
                            );
                        }
                    }
                }
                con.Close();
            }

            return await Task.FromResult(res);

        }

        public async Task<ICollection<Sales>> GetInfoSalesClient(string data)
        {
            List<Sales> res = new List<Sales>();

            return await Task.FromResult(res);

        }

        public async Task<Task> DeleteClient(CompanyDto company, ClientDto client, UserDto user)
        {            
            string companyName = company.Name;     

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "update [Clients] " +
                        "set [Status]=@Status,[UpdateByUserId]=@UserId,UpdatedAt=@date " +
                        "where [Clients].Id=@ClientId ";                    
                    cmd.Parameters.AddWithValue("ClientId", client.Id);                
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Deleted);             

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                  
                }
                con.Close();
            }
            return Task.CompletedTask;
        }
    }
}
