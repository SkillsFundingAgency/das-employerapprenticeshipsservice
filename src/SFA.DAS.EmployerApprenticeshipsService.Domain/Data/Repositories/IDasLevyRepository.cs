using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task CreateEmployerDeclaration(DasDeclaration dasDeclaration, string empRef, long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
        Task ProcessDeclarations();
        Task<List<TransactionLine>> GetTransactions(long accountId);
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
        Task CreateNewPeriodEnd(PeriodEnd periodEnd);
        Task<PeriodEnd> GetLatestPeriodEnd();
        Task<List<TransactionLine>> GetTransactionsByDateRange(long accountId, DateTime fromDate, DateTime toDate);
        Task CreatePaymentData(Payment payment, long accountId, string periodEnd, string providerName, string courseName);
        Task<Payment> GetPaymentData(Guid paymentId);
        Task ProcessPaymentData();
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(string empRef);

    }
}
