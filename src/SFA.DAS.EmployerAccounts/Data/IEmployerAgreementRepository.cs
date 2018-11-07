using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
    }
}