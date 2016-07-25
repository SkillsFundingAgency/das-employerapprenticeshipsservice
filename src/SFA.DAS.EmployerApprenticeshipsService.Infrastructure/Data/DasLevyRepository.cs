using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data
{
    public class DasLevyRepository : IDasLevyRepository
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public DasLevyRepository(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                var sql = @"select ld.* from  [dbo].[LevyDeclaration] ld where ld.empRef = @EmpRef and ld.SubmissionId = @Id";
                var account = connection.QueryFirstOrDefault(sql, new { Id = id, EmpRef = empRef });

                connection.Close();
                if (account == null)
                {
                    return account;
                }
                else
                {
                    return new DasDeclaration
                    {
                        Amount = account.Amount,
                        Id = account.SubmissionId,
                        SubmissionType = account.SubmissionType,
                        Date = account.SubmissionDate
                    };
                }

            }

        }

        public async Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef)
        {
            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();
                var sql = @"insert into [dbo].LevyDeclaration (Amount, empRef, SubmissionDate, SubmissionId, SubmissionType) values (@Amount, @EmpRef, @SubmissionDate, @SubmissionId, @SubmissionType)";

                await connection.ExecuteAsync(sql, new { dasDeclaration.Amount, SubmissionDate = dasDeclaration.Date, SubmissionId = dasDeclaration.Id, EmpRef = empRef, dasDeclaration.SubmissionType });
                connection.Close();
            }
        }

        public async Task<DasEnglishFractions> GetEmployerFraction(DateTime dateCalculated, string empRef)
        {
            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                var sql = @"select ef.* from  [dbo].[EnglishFraction] ef where ef.EmpRef = @EmpRef and ef.DateCalculated = @DateCalculated";
                var fraction = connection.QueryFirstOrDefault(sql, new { DateCalculated = dateCalculated, EmpRef = empRef });

                connection.Close();
                if (fraction == null)
                {
                    return fraction;
                }
                else
                {
                    return new DasEnglishFractions
                    {
                        Amount = fraction.Amount,
                        DateCalculated = fraction.DateCalculated
                    };
                }

            }

        }

        public async Task CreateEmployerFraction(DasEnglishFractions fractions, string empRef)
        {
            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();
                var sql = @"insert into [dbo].EnglishFraction (EmpRef, DateCalculated, Amount) values (@EmpRef, @DateCalculated, @Amount)";

                await connection.ExecuteAsync(sql, new { fractions.Amount, EmpRef = empRef, fractions.DateCalculated });
                connection.Close();
            }
        }

        public async Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(int accountId)
        {
            var declarations = new List<LevyDeclarationView>();

            using (var connection = new SqlConnection(_configuration.Employer.DatabaseConnectionString))
            {
                await connection.OpenAsync();

                var sql = @"SELECT * from [dbo].[GetLevyDeclarations] WHERE [AccountId] = @accountId ORDER BY [SubmissionDate] ASC";
                declarations = connection.Query<LevyDeclarationView>(sql, new { accountId = accountId }).ToList();

                connection.Close();
            }

            return declarations;
        }
    }
}

