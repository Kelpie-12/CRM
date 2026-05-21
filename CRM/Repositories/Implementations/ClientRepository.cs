using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.DTO;
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
            Client res = null;
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT 1 FROM [Clients] WHERE [Clients].FirstName=@FirstName and [Clients].LastName=@LastName) " +
                                      "BEGIN " +
                                      "INSERT INTO [Clients] ([Clients].FirstName,[Clients].LastName,[Clients].[Status],[Clients].CreatedByUserId) " +
                                      "VALUES (@FirstName,@LastName,@Status,@UserId); " +
                                      "SELECT Clients.Id,Clients.FirstName,Clients.LastName,[Status].[Name] " +
                                      "from [Clients],[Status] " +
                                      "where Clients.Id=(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                                      "and [Status].Id=[Clients].[Status] " +
                                      "END " +
                                      "ELSE BEGIN " +
                                      "SELECT 'Такой пользователь уже существует' AS Result; " +
                                      "END ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", 1);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) != "Такой пользователь уже существует")
                            {
                                res = new Client();
                                res.Id = reader.GetInt32(0);
                                res.FirstName = reader.GetString(1);
                                res.LastName = reader.GetString(2);
                                res.Status = reader.GetString(3);
                            }

                        }
                    }
                }
                con.Close();
            }
            return res;
        }

        public async Task<Sales> CreateSalesForClient(string data)
        {
            Sales sales = new Sales();


            return await Task.FromResult(sales);

            throw new NotImplementedException();
        }

        public async Task<Client> EditingClient(CompanyDto company, ClientDto client, UserDto user)
        {
            Client res = new Client();
            using (SqlConnection con = CreateConnection(company.Name))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "update [Clients] " +
                        "set [FirstName]=@FirstName, [LastName]=@LastName, [Status]=@Status,[UpdateByUserId]=UserId " +
                        "where [Clients].Id=(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                        "select Clients.Id,Clients.FirstName,Clients.LastName,[Status].[Name] " +
                        "from [Clients],[Status] " +
                        "where Clients.Id=(select Clients.Id from Clients where Clients.FirstName=@FirstName and Clients.LastName=@LastName) " +
                        "and [Status].Id=[Clients].[Status] ";
                    cmd.Parameters.AddWithValue("FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("LastName", client.LastName);
                    //cmd.Parameters.AddWithValue("ClientId", client.Id);
                    cmd.Parameters.AddWithValue("UserId", user.Id);
                    cmd.Parameters.AddWithValue("Status", 1);


                    //cmd.Parameters.AddWithValue("CompanyId", "-1");
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    //cmd.ExecuteNonQuery(); 
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (reader.GetString(0) == "Успех")
                            {
                                res.Id = reader.GetInt32(0);
                                res.FirstName = reader.GetString(1);
                                res.LastName = reader.GetString(2);
                                res.Status = reader.GetString(3);
                            }
                        }
                    }
                }
                con.Close();
            }
            return res;

        }

        public Task<Client> GetInfoClient(ClientDto client)
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

        public async Task<ICollection<Client>> GetInfoListClients(string data)
        {
            List<Client> res = new List<Client>();


            return await Task.FromResult(res);

        }

        public async Task<ICollection<Sales>> GetInfoSalesClient(string data)
        {
            List<Sales> res = new List<Sales>();

            return await Task.FromResult(res);

        }
    }
}
