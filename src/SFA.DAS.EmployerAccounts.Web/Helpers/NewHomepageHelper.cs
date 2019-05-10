using SFA.DAS.EAS.Portal.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.HashingService;
using System.Linq;
using Account = SFA.DAS.EAS.Portal.Types.Account;

namespace SFA.DAS.EmployerAccounts.Web.Helpers
{
    public class NewHomepageHelper
    {
        private readonly IHashingService _hashingService;

        public NewHomepageHelper(IHashingService hashingService)
        {
            _hashingService = hashingService;
        }

        internal Account ConvertFromOldModelToNewModel(OrchestratorResponse<AccountDashboardViewModel> _oldAccountInfo)
        {
            var oldAccountInfo = _oldAccountInfo.Data;
            return new Account
            {
                Id = oldAccountInfo.HashedAccountId,
                Name = oldAccountInfo.Account.Name,
                Organisations = oldAccountInfo.Account.AccountLegalEntities.Select(ConvertFromAccountLegalEntityToOrganisation).ToArray(),
            };
        }

        internal Organisation ConvertFromAccountLegalEntityToOrganisation(AccountLegalEntity ale)
        {
            return new Organisation
            {
                Id = ale.LegalEntityId,
                Agreements = ale.Agreements.Select(ConvertFromEmployerAgreementToAgreement).ToArray(),
            };
        }

        internal Agreements ConvertFromEmployerAgreementToAgreement(EmployerAgreement employerAgreement)
        {
            return new Agreements
            {
                HashedAgreementId = _hashingService.HashValue(employerAgreement.Id),
                Version = employerAgreement.TemplateId,
                IsPending = employerAgreement.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending? true : false
            };
        }
    }
}