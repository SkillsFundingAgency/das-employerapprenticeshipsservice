using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<AccountDetailViewModel> GetAccount(string hashedAccountId);
        Task<AccountDetailViewModel> GetAccount(long accountId);
        Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId);
        Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId);
        Task Ping();
    }
}