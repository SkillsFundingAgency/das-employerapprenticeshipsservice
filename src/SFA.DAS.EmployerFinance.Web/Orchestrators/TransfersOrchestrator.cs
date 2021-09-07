using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.HashingService;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class TransfersOrchestrator
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHashingService _hashingService;
        private readonly ILevyTransferMatchingService _levyTransferMatchingService;
        private readonly IAccountApiClient _accountApiClient;
        private readonly ICohortsService _cohortsService;

        protected TransfersOrchestrator()
        {

        }

        public TransfersOrchestrator(
            IAuthorizationService authorizationService,
            IHashingService hashingService,
            ILevyTransferMatchingService levyTransferMatchingService,
            IAccountApiClient accountApiClient,
            ICohortsService cohortsService)
        {
            _authorizationService = authorizationService;
            _hashingService = hashingService;
            _levyTransferMatchingService = levyTransferMatchingService;
            _accountApiClient = accountApiClient;
            _cohortsService = cohortsService;
        }

        public async Task<OrchestratorResponse<TransfersIndexViewModel>> Index(string hashedAccountId)
        {
            var accountDetail = await _accountApiClient.GetAccount(hashedAccountId).ConfigureAwait(false);
            var cohortsCount = await _cohortsService.GetCohortsCount(accountDetail.AccountId).ConfigureAwait(false);

            Enum.TryParse(accountDetail.ApprenticeshipEmployerType, true, out ApprenticeshipEmployerType employerType);

            if (employerType != ApprenticeshipEmployerType.Levy || cohortsCount > 0)
            {
                return new OrchestratorResponse<TransfersIndexViewModel>()
                {
                    Data = new TransfersIndexViewModel()
                    {
                       CanViewTransfersSection = false
                    }
                };
            }

            return await GenerateTransfersViewIndexModelForLevyAccounts(hashedAccountId);
        }

        private async Task<OrchestratorResponse<TransfersIndexViewModel>> GenerateTransfersViewIndexModelForLevyAccounts(string hashedAccountId)
        {
            bool renderCreateTransfersPledgeButton =
                await _authorizationService.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor);

            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var pledgesCount = await _levyTransferMatchingService.GetPledgesCount(accountId);

            var viewModel = new OrchestratorResponse<TransfersIndexViewModel>()
            {
                Data = new TransfersIndexViewModel()
                {
                    RenderCreateTransfersPledgeButton = renderCreateTransfersPledgeButton,
                    PledgesCount = pledgesCount,
                    CanViewTransfersSection = true
                }
            };

            return viewModel;
        }
    }
}