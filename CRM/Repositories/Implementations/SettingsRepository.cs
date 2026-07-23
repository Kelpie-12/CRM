using System.Collections.Generic;
using System.Data.SqlClient;
using CRM.Model;
using CRM.Model.DTO;
using CRM.Model.LocalStorage;

namespace CRM.Repositories.Implementations
{
    public class SettingsRepository : BaseRepository, ISettingsRepository
    {
        private static readonly HashSet<string> SystemRoleNames = new(StringComparer.OrdinalIgnoreCase)
        {
            Roles.Admin,
            Roles.Employee,
            Roles.Ceo
        };

        private static readonly HashSet<string> AllowedPermissionNames = new(StringComparer.OrdinalIgnoreCase)
        {
            Permissions.Read,
            Permissions.Create,
            Permissions.Update,
            Permissions.Delete
        };

        public SettingsRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<CompanySettingsDto> GetSettings(int companyId, string companyName)
        {
            CompanySettingsDto settings = new CompanySettingsDto
            {
                CompanyId = companyId,
                CompanyName = companyName,
                Description = string.Empty,
                CashboxId = 0,
                CashboxName = string.Empty,
                Cashboxes = new List<CashboxDto>(),
                Positions = new List<CompanyRoleDto>()
            };

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT TOP 1 ISNULL([Description], ''), ISNULL([Name], @CompanyName) " +
                        "FROM [Enterprise]";
                    cmd.Parameters.AddWithValue("CompanyName", companyName);

                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows && reader.Read())
                        {
                            settings.Description = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                            settings.CompanyName = reader.IsDBNull(1) ? companyName : reader.GetString(1);
                        }
                        reader.Close();
                    }
                    catch
                    {
                        // Enterprise table may not exist yet in some company databases.
                    }
                }

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Cashbox ORDER BY Id";
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            settings.Cashboxes.Add(new CashboxDto
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1)
                            });
                        }
                        reader.Close();
                    }
                    catch
                    {
                        settings.Cashboxes.Add(new CashboxDto { Id = 1, Name = "Main" });
                    }
                }

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT TOP 1 CAST([Value] AS INT) " +
                        "FROM [Settings] WHERE [Key]='ConnectedCashboxId'";
                    try
                    {
                        object? result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            settings.CashboxId = Convert.ToInt32(result);
                        }
                    }
                    catch
                    {
                        // Settings table may not exist yet.
                    }
                }

                using (SqlCommand cmd = con.CreateCommand())
                {
         
                    cmd.CommandText =
                        "SELECT *  " +
                        "FROM [Role] ";
                    try
                    {
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {                               
                                settings.Positions.Add(new CompanyRoleDto() 
                                { 
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }

                    }
                    catch
                    {
                        // Settings table may not exist yet.
                    }
                }


                con.Close();
            }


            if (settings.CashboxId == 0 && settings.Cashboxes.Count > 0)
            {
                settings.CashboxId = 1;// settings.Cashboxes[0].Id;
            }

            settings.CashboxName = settings.Cashboxes
                .FirstOrDefault(c => c.Id == settings.CashboxId)?.Name
                ?? settings.Cashboxes.FirstOrDefault()?.Name
                ?? "Main";

            return settings;
        }

        public async Task<bool> UpdateCompanyName(int companyId, string companyName, string newName)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "UPDATE [Enterprise] SET [Name]=@NewName";
                    cmd.Parameters.AddWithValue("NewName", newName);
                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch
                    {
                        // Ignore if Enterprise table is missing.
                    }
                }
                con.Close();
            }

            using (SqlConnection con = CreateConnection())
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = "USE [CRM] UPDATE [Company] SET [Name]=@NewName WHERE [Id]=@CompanyId";
                    cmd.Parameters.AddWithValue("NewName", newName);
                    cmd.Parameters.AddWithValue("CompanyId", companyId);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }

        public async Task<bool> UpdateCompanyDescription(string companyName, string description)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "IF EXISTS (SELECT 1 FROM [Enterprise]) " +
                        "UPDATE [Enterprise] SET [Description]=@Description " +
                        "ELSE INSERT INTO [Enterprise] ([Name],[Description]) VALUES (@CompanyName,@Description)";
                    cmd.Parameters.AddWithValue("Description", description ?? string.Empty);
                    cmd.Parameters.AddWithValue("CompanyName", companyName);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }

        public async Task<bool> UpdateConnectedCashbox(string companyName, int cashboxId)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText =
                        "IF EXISTS (SELECT 1 FROM [Settings] WHERE [Key]='ConnectedCashboxId') " +
                        "UPDATE [Settings] SET [Value]=@CashboxId WHERE [Key]='ConnectedCashboxId' " +
                        "ELSE INSERT INTO [Settings] ([Key],[Value]) VALUES ('ConnectedCashboxId', @CashboxId)";
                    cmd.Parameters.AddWithValue("CashboxId", cashboxId);
                    await cmd.ExecuteNonQueryAsync();
                }
                con.Close();
            }

            return true;
        }

        public async Task<CompanyRoleDto?> CreateRole(string companyName, string roleName, ICollection<string> permissions)
        {
            string normalizedName = roleName.Trim();
            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return null;
            }

            ICollection<string> normalizedPermissions = NormalizePermissions(permissions);
            int roleId = -1;

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();


                using (SqlCommand existsCmd = con.CreateCommand())
                {
                    existsCmd.CommandText = "SELECT TOP 1 Id FROM [Role] WHERE [Name]=@RoleName";
                    existsCmd.Parameters.AddWithValue("RoleName", normalizedName);
                    object? existing = await existsCmd.ExecuteScalarAsync();
                    if (existing != null && existing != DBNull.Value)
                    {
                        con.Close();
                        return null;
                    }
                }

                using (SqlCommand insertCmd = con.CreateCommand())
                {
                    insertCmd.CommandText = "INSERT INTO [Role] ([Name]) VALUES (@RoleName); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    insertCmd.Parameters.AddWithValue("RoleName", normalizedName);
                    roleId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                }


                con.Close();
            }



            return new CompanyRoleDto
            {
                Id = roleId,
                Name = normalizedName
            };
        }

        public async Task<CompanyRoleDto?> UpdateRole(
            string companyName,
            int roleId,
            string? roleName,
            ICollection<string> permissions)
        {
            ICollection<string> normalizedPermissions = NormalizePermissions(permissions);
            string finalName = string.Empty;

            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();


                string? currentName = null;
                using (SqlCommand loadCmd = con.CreateCommand())
                {
                    loadCmd.CommandText = "SELECT TOP 1 [Name] FROM [Role] WHERE Id=@RoleId";
                    loadCmd.Parameters.AddWithValue("RoleId", roleId);
                    object? result = await loadCmd.ExecuteScalarAsync();
                    if (result == null || result == DBNull.Value)
                    {
                        con.Close();
                        return null;
                    }

                    currentName = Convert.ToString(result);
                }

                bool isSystem = IsSystemRole(currentName!);
                finalName = currentName!;

                if (!isSystem && !string.IsNullOrWhiteSpace(roleName))
                {
                    finalName = roleName.Trim();
                    using (SqlCommand duplicateCmd = con.CreateCommand())
                    {
                        duplicateCmd.CommandText =
                            "SELECT TOP 1 Id FROM [Role] WHERE [Name]=@RoleName AND Id <> @RoleId";
                        duplicateCmd.Parameters.AddWithValue("RoleName", finalName);
                        duplicateCmd.Parameters.AddWithValue("RoleId", roleId);
                        object? duplicate = await duplicateCmd.ExecuteScalarAsync();
                        if (duplicate != null && duplicate != DBNull.Value)
                        {
                            con.Close();
                            return null;
                        }
                    }

                    using (SqlCommand renameCmd = con.CreateCommand())
                    {
                        renameCmd.CommandText = "UPDATE [Role] SET [Name]=@RoleName WHERE Id=@RoleId";
                        renameCmd.Parameters.AddWithValue("RoleName", finalName);
                        renameCmd.Parameters.AddWithValue("RoleId", roleId);
                        await renameCmd.ExecuteNonQueryAsync();
                    }

                }


                con.Close();
            }

            return new CompanyRoleDto
            {
                Id = roleId,
                Name = finalName
            };
        }

        public async Task<bool> DeleteRole(string companyName, int roleId)
        {
            using (SqlConnection con = CreateConnection(companyName))
            {
                con.Open();

                string? roleName = null;
                using (SqlCommand loadCmd = con.CreateCommand())
                {
                    loadCmd.CommandText = "SELECT TOP 1 [Name] FROM [Role] WHERE Id=@RoleId";
                    loadCmd.Parameters.AddWithValue("RoleId", roleId);
                    object? result = await loadCmd.ExecuteScalarAsync();
                    if (result == null || result == DBNull.Value)
                    {
                        con.Close();
                        return false;
                    }

                    roleName = Convert.ToString(result);
                }

                if (IsSystemRole(roleName!))
                {
                    con.Close();
                    return false;
                }

                using (SqlCommand usageCmd = con.CreateCommand())
                {
                    usageCmd.CommandText = "SELECT COUNT(1) FROM Employee WHERE RoleId=@RoleId";
                    usageCmd.Parameters.AddWithValue("RoleId", roleId);
                    int usageCount = Convert.ToInt32(await usageCmd.ExecuteScalarAsync());
                    if (usageCount > 0)
                    {
                        con.Close();
                        return false;
                    }
                }

                using (SqlCommand deletePermissionsCmd = con.CreateCommand())
                {
                    deletePermissionsCmd.CommandText = "DELETE FROM [RolePermissions] WHERE RoleId=@RoleId";
                    deletePermissionsCmd.Parameters.AddWithValue("RoleId", roleId);
                    await deletePermissionsCmd.ExecuteNonQueryAsync();
                }

                using (SqlCommand deleteRoleCmd = con.CreateCommand())
                {
                    deleteRoleCmd.CommandText = "DELETE FROM [Role] WHERE Id=@RoleId";
                    deleteRoleCmd.Parameters.AddWithValue("RoleId", roleId);
                    await deleteRoleCmd.ExecuteNonQueryAsync();
                }

                con.Close();
            }

            return true;
        }



        private static ICollection<string> NormalizePermissions(ICollection<string> permissions)
        {
            return permissions
                .Where(permission => AllowedPermissionNames.Contains(permission))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static bool IsSystemRole(string roleName)
        {
            return SystemRoleNames.Contains(roleName);
        }
    }
}
