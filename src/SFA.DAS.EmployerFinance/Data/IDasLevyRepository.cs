using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IDasLevyRepository
    {
        Task CreateEmployerDeclarations(IEnumerable<DasDeclaration> dasDeclaration, string empRef, long accountId);
        Task CreateNewPeriodEnd(PeriodEnd periodEnd);
        Task CreatePayments(IEnumerable<PaymentDetails> payments);
        Task<ISet<Guid>> GetAccountPaymentIds(long accountId);
        Task<IEnumerable<long>> GetEmployerDeclarationSubmissionIds(string empRef);
        Task<DasDeclaration> GetLastSubmissionForScheme(string empRef);
        Task<PeriodEnd> GetLatestPeriodEnd();
        Task<DasDeclaration> GetSubmissionByEmprefPayrollYearAndMonth(string empRef, string payrollYear, short payrollMonth);
        Task ProcessDeclarations(long accountId, string empRef);
        Task ProcessPaymentData(long accountId);
    }
}
