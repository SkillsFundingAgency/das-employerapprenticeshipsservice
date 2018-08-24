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
        Task<ICollection<T>> GetAccountProviderPaymentsByDateRange<T>(
            long accountId, long ukprn, DateTime fromDate, DateTime toDate)
            where T : TransactionLine;


        Task<ICollection<T>> GetAccountCoursePaymentsByDateRange<T>(
            long accountId, long ukprn, string courseName, int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
            where T : TransactionLine;
    }
}
