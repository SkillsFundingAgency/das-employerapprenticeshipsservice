using SFA.DAS.EmployerFinance.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasLevyService
    {
        Task<ICollection<TransactionLine>> GetAccountTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);

        Task<int> GetPreviousAccountTransaction(long AccountId, DateTime FromDate);
		
		Task<ICollection<T>> GetAccountProviderPaymentsByDateRange<T>(
            long accountId, long ukprn, DateTime fromDate, DateTime toDate)
            where T : TransactionLine;


        Task<ICollection<T>> GetAccountCoursePaymentsByDateRange<T>(
            long accountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
            where T : TransactionLine;
    }
}
