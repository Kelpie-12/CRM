using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using CRM.Model;
using CRM.Model.Data;
using CRM.Model.Data.Response;
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
        public async Task<ClientResponse> CreateClient(CompanyDto company, ClientDto client, UserDto user)
        {
            ClientResponse res = new ClientResponse()
            {
                Error = "none",
                Message = "Client was created!",
                Client = new Client()
            };
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Clients].FirstName=@FirstName and [Clients].LastName=@LastName) " +
                                      "BEGIN " +
                                      "INSERT INTO [Clients] ([Clients].FirstName,[Clients].LastName,[Clients].[Status],[Clients].CreatedByUserId) " +
                                      "VALUES (@FirstName,@LastName,@Status,@UserId); " +
                                      "SELECT Clients.FirstName,Clients.Id,Clients.LastName,Clients.[Status] " +
                                      "from [Clients] " +
                                      "where Clients.Id=(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                                     // "and [Status].Id=[Clients].[Status] " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Client already exist' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Active);
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        //cmd.ExecuteNonQuery(); 
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetString(0) != "Client already exist")
                                {
                                    res.Client.FirstName = reader.GetString(0);
                                    res.Client.Id = reader.GetInt32(1);
                                    res.Client.LastName = reader.GetString(2);
                                    res.Client.Status = reader.GetInt32(3);
                                }
                                else
                                {
                                    res.Message = reader.GetString(0);
                                    res.Client.Id = -1;
                                    res.Error = "error_client_already_exist";
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        res.Message = e.Message;
                        res.Error = "error_create_client";
                        res.Client.Id = -1;
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

        }

        public async Task<ClientResponse> UpdateClient(CompanyDto company, ClientDto client, UserDto user)

        {
            ClientResponse res = new ClientResponse()
            {
                Error = "none",
                Message = "Client was created!",
                Client = new Client()
            };

            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "update [Clients] " +
                        "set [FirstName]=@FirstName, [LastName]=@LastName, [Status]=@Status,[UpdateByUserId]=@UserId,UpdatedAt=@date " +
                        "where [Clients].Id=@ClientId " +
                        //"(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                        "select Clients.Id,Clients.FirstName,Clients.LastName,Clients.[Status] " +
                        "from [Clients] " +
                        "where Clients.Id=@ClientId " +
                        //"(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                       // "and [Status].Id=[Clients].[Status] ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("ClientId", client.Id);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    //cmd.Parameters.AddWithValue("ClientId", client.Id);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", Status.Active);
                    cmd.Parameters.AddWithValue("date", DateTime.Now);
                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        //cmd.ExecuteNonQuery(); 
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Client.Id = reader.GetInt32(0);
                                res.Client.FirstName = reader.GetString(1);
                                res.Client.LastName = reader.GetString(2);
                                res.Client.Status = reader.GetInt32(3);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        res.Message = e.Message;
                        res.Error = "error_update_client";
                        res.Client.Id = -1;
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
                Status = 1
            };
            return Task.FromResult(res);
        }

        public async Task<ICollection<Client>> GetInfoListClients(int companyId, int status)
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
                con.Close();
            }
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "select Clients.Id,Clients.FirstName,Clients.LastName,Clients.Status from Clients where Clients.Status=@Status";

                    cmd.Parameters.AddWithValue("@Status", status);

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
                                    LastName = reader.GetString(2),
                                    Status=reader.GetInt32(3)
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
