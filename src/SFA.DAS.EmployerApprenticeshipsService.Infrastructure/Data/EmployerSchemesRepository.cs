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
    public class EmployerSchemesRepository : IEmployerSchemesRepository
    {
        readonly string _connectionString = String.Empty;
        public EmployerSchemesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Schemes> GetSchemesByEmployerId(int employerId)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = @"select a.* from [dbo].[Paye] a where a.AccountId = @Id";
                var schemes = connection.Query<Scheme>(sql, new { Id = employerId });

                connection.Close();
                return new Schemes { SchemesList =  (List<Scheme>) schemes};
            }

        }

     
    }
}
