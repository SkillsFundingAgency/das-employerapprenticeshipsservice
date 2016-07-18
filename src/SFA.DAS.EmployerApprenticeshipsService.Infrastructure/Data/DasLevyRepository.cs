﻿using System;
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

        public async Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef)
        {
            using (var connection = new SqlConnection(_connectionString))
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
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"insert into [dbo].LevyDeclaration (Amount, empRef, SubmissionDate, SubmissionId, SubmissionType) values (@Amount, @EmpRef, @SubmissionDate, @SubmissionId, @SubmissionType)";

                await connection.ExecuteAsync(sql, new { dasDeclaration.Amount, SubmissionDate = dasDeclaration.Date, SubmissionId = dasDeclaration.Id, EmpRef = empRef, dasDeclaration.SubmissionType });
                connection.Close();
            }
        }

        public async Task<DasEnglishFractions> GetEmployerFraction(DateTime dateCalculated, string empRef)
        {
            using (var connection = new SqlConnection(_connectionString))
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
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var sql = @"insert into [dbo].EnglishFraction (EmpRef, DateCalculated, Amount) values (@EmpRef, @DateCalculated, @Amount)";

                await connection.ExecuteAsync(sql, new { fractions.Amount, EmpRef = empRef, fractions.DateCalculated });
                connection.Close();
            }
        }


    }
}

