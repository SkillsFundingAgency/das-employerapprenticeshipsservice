using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client
{
    public interface IAccountApiClient
    {
        Task<AccountDetailViewModel> GetAccount(string hashedAccountId);
        Task<AccountDetailViewModel> GetAccount(long accountId);
        Task<ICollection<TeamMemberViewModel>> GetAccountUsers(string accountId);
        Task<ICollection<TeamMemberViewModel>> GetAccountUsers(long accountId);
        Task<EmployerAgreementView> GetEmployerAgreement(string accountId, string legalEntityId, string agreementId);
        Task<ICollection<ResourceViewModel>> GetLegalEntitiesConnectedToAccount(string accountId);
        Task<LegalEntityViewModel> GetLegalEntity(string accountId, long id);
        Task<ICollection<LevyDeclarationViewModel>> GetLevyDeclarations(string accountId);
        Task<PagedApiResponseViewModel<AccountWithBalanceViewModel>> GetPageOfAccounts(int pageNumber = 1, int pageSize = 1000, DateTime? toDate = null);
        Task<ICollection<ResourceViewModel>> GetPayeSchemesConnectedToAccount(string accountId);
        Task<T> GetResource<T>(string uri) where T : IAccountResource;
        Task<TransactionsViewModel> GetTransactions(string accountId, int year, int month);
        Task<ICollection<TransactionSummaryViewModel>> GetTransactionSummary(string accountId);
        Task<ICollection<TransferConnectionViewModel>> GetTransferConnections(string accountId);
        Task<ICollection<AccountDetailViewModel>> GetUserAccounts(string userId);
    }
}