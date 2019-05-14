using SFA.DAS.EAS.Portal.Client.Models;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;
using SFA.DAS.EAS.Portal.Types;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.HashingService;
using System;
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
            var org = new Organisation();
            org.Agreements = oldAccountInfo.PendingAgreements.Select(ConvertFromPendingAgreementsViewModelToAgreement).ToList();

            return new Account
            {
                Id = oldAccountInfo.HashedAccountId,
                Name = oldAccountInfo.Account.Name,
                Organisations = new Organisation[]
                {
                    org
                }
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

        internal Agreement ConvertFromEmployerAgreementToAgreement(EmployerAgreement employerAgreement)
        {
            return new Agreement
            {
                HashedAgreementId = _hashingService.HashValue(employerAgreement.Id),
                Version = employerAgreement.TemplateId,
                IsPending = employerAgreement.StatusId == Models.EmployerAgreement.EmployerAgreementStatus.Pending? true : false
            };
        }
        internal Agreement ConvertFromPendingAgreementsViewModelToAgreement(PendingAgreementsViewModel pavm)
        {
            return new Agreement
            {
                HashedAgreementId = pavm.HashedAgreementId,
                IsPending = true
            };
        }

        internal Account ConvertFromPortalAccountToNewModel(AccountDto result)
        {
            var newAcc = new Account
            {
                Id = _hashingService.HashValue(result.AccountId),
                Deleted = result.Deleted,
                Organisations = result.AccountLegalEntities.Select(ale => ConvertFromPortalAccountLegalEntitiesToOrganisation(ale)).ToList(),
                Name = result.AccountLegalEntities.FirstOrDefault().LegalEntityName
            };
            return newAcc;
        }

        internal Organisation ConvertFromPortalAccountLegalEntitiesToOrganisation(IAccountLegalEntityDto<IReservedFundingDto> ale)
        {
            return new Organisation
            {
                Id = ale.AccountLegalEntityId,
                ReserveFundings = ale.ReservedFundings.Select(ConvertFromPortalReservedFundingsToReservedFundings).ToList()
            };
        }

        internal ReserveFunding ConvertFromPortalReservedFundingsToReservedFundings(IReservedFundingDto reservedFunding)
        {
            return new ReserveFunding
            {
                CourseCode = reservedFunding.CourseId,
                CourseName = reservedFunding.CourseName,
                StartDate = reservedFunding.StartDate,
                EndDate = reservedFunding.EndDate,
                ReservationId = reservedFunding.ReservationId
            };
        }
    }
}