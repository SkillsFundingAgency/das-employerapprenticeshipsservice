using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IDasLevyRepository
    {
        Task<DasDeclaration> GetEmployerDeclaration(string id, string empRef);
        Task<IEnumerable<string>> GetEmployerDeclarationIds(string empRef);
        Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> dasDeclaration, string empRef, long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId);
        Task<List<LevyDeclarationView>> GetAccountLevyDeclarations(long accountId, string payrollYear, short payrollMonth);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
        Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth);
        Task ProcessDeclarations(long accountId, string empRef);
       
     
        Task<List<AccountBalance>> GetAccountBalances(List<long> accountIds);
        Task CreateNewPeriodEnd(PeriodEnd periodEnd);
        Task<PeriodEnd> GetLatestPeriodEnd();
     
        Task CreatePaymentData(IEnumerable<PaymentDetails> payments);
        Task<Payment> GetPaymentData(Guid paymentId);
        Task ProcessPaymentData();
        Task<IEnumerable<DasEnglishFraction>> GetEnglishFractionHistory(long accountId, string empRef);

    }
}
