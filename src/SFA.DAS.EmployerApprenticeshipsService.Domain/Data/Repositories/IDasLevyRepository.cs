using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
        Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth);
        Task ProcessDeclarations();
        Task<List<TransactionLine>> GetTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
        Task<int> GetPreviousTransactionsCount(long accountId, DateTime fromDate);
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
        Task CreateNewPeriodEnd(PeriodEnd periodEnd);
        Task<PeriodEnd> GetLatestPeriodEnd();
        Task<List<TransactionLine>> GetTransactionDetailsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
        Task CreatePaymentData(PaymentDetails paymentDetails);
        Task<Payment> GetPaymentData(Guid paymentId);
        Task ProcessPaymentData();
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);

    }
}
