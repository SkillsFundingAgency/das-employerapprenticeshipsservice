using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class EmployerAccountRepository : IEmployerAccountRepository
    {
        readonly string _connectionString = String.Empty;
        public EmployerAccountRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Account> GetAccountById(int id)
        {
        
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = @"select a.* from [dbo].[Account] a where a.Id = @Id";
                var account = await connection.QueryFirstOrDefaultAsync<Account>(sql, new { Id = id });

                connection.Close();
                 return account;
                //return new Account { Id = account.Id, Name = account.Name};
            }

        }
    }
}

