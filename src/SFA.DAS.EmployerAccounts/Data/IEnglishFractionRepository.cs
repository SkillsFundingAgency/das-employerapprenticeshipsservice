using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEnglishFractionRepository
    {
        Task<IEnumerable<DasEnglishFraction>> GetCurrentFractionForSchemes(
            long accountId,
            IEnumerable<string> employerReferences);
    }

    public class EnglishFractionRepository : IEnglishFractionRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public EnglishFractionRepository(Lazy<EmployerFinanceDbContext> db)
        {
            _db = db;
        }

        public async Task<IEnumerable<DasEnglishFraction>> GetCurrentFractionForSchemes(long accountId, IEnumerable<string> employerReferences)
        {
            var currentFractions = new List<DasEnglishFraction>();

            foreach (var employerReference in employerReferences)
            {
                var parameters = new DynamicParameters();

                parameters.Add("@accountId", accountId, DbType.Int64);
                parameters.Add("@empRef", employerReference, DbType.String);

                var currentFraction = await _db.Value.Database.Connection.QueryAsync<DasEnglishFraction>(
                    sql: "[employer_financial].[GetCurrentFractionForScheme]",
                    param: parameters,
                    transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                    commandType: CommandType.StoredProcedure);

                currentFractions.Add(currentFraction.FirstOrDefault());
            }

            return currentFractions;
        }

    }
}
