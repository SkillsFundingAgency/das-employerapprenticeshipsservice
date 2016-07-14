using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class DasLevyRepository : IDasLevyRepository
    {
        readonly string _connectionString = String.Empty;
        public DasLevyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async  Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = @"select ld.* from  [dbo].[LevyDeclaration] ld where ld.empRef = @EmpRef and ld.SubmissionId = @Id";
                var account = connection.QueryFirstOrDefault(sql, new { Id = id, EmpRef = empRef });
                
                connection.Close();
                return account;
            }

        }

        public async Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"insert into [dbo].LevyDeclaration (Amount, PayeId, SubmissionDate, SubmissionId, SubmissionType) values (@Amount, @PayeId, @SubmissionDate, @SubmissionId, @SubmissionType)";
                
                await connection.ExecuteAsync(sql, new {dasDeclaration.Amount, SubmissionDate = dasDeclaration.Date, SubmissionId = dasDeclaration.Id, PayeId = empRef, dasDeclaration.SubmissionType});
                connection.Close();
            }
        }

        public Task<DasEnglishFractions> GetEmployerFraction(DateTime dateCalculated, string empRef)
        {
            throw new NotImplementedException();
        }

        public Task CreateEmployerFraction(DasEnglishFractions fractions, string empRef)
        {
            throw new NotImplementedException();
        }

     
    }
}
