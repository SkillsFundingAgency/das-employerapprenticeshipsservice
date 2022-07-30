using Newtonsoft.Json;
using SFA.DAS.EAS.Finance.Api.Types;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public class EmployerFinanceApiService : IEmployerFinanceApiService
    {
        private readonly ILog _log;
        private readonly IEmployerFinanceApiClient _employerFinanceApiClient;

        public EmployerFinanceApiService(IEmployerFinanceApiClient employerFinanceApiClient, ILog log)
        {
            _employerFinanceApiClient = employerFinanceApiClient;
            _log = log;            
        }       

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string hashedAccountId)
        {
            return await _employerFinanceApiClient.GetLevyDeclarations(hashedAccountId);       
        }

        public async Task<ICollection<LevyDeclarationViewModel>> GetLevyForPeriod(string hashedAccountId, string payrollYear, short payrollMonth)
        {
            return await _employerFinanceApiClient.GetLevyForPeriod(hashedAccountId, payrollYear, payrollMonth);
        }

        public async Task<FinanceStatisticsViewModel> GetStatistics(CancellationToken cancellationToken = default)
        {
            return await _employerFinanceApiClient.GetFinanceStatistics();
        }

        public async Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month)
        {
            return await _employerFinanceApiClient.GetTransactions(accountId, year, month);
        }

        public Task<AccountDetailViewModel> GetAccount(string hashedAccountId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetAccounts(string toDate, int pageSize, int pageNumber, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId)
        {
            return await _employerFinanceApiClient.GetTransactionSummary(accountId);
        }


        public Task<dynamic> Redirect(string url, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
       
    }
}
