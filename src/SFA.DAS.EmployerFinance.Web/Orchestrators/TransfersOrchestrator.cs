using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class TransfersOrchestrator
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHashingService _hashingService;
        private readonly ILevyTransferMatchingService _levyTransferMatchingService;

        protected TransfersOrchestrator()
        {

        }

        public TransfersOrchestrator(
            IAuthorizationService authorizationService,
            IHashingService hashingService,
            ILevyTransferMatchingService levyTransferMatchingService)
        {
            _authorizationService = authorizationService;
            _hashingService = hashingService;
            _levyTransferMatchingService = levyTransferMatchingService;
        }

        public async Task<OrchestratorResponse<TransfersIndexViewModel>> Index(string hashedAccountId)
        {
            bool renderCreateTransfersPledgeButton = true; //await _authorizationService.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor);

            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var pledgesCount = await _levyTransferMatchingService.GetPledgesCount(accountId);
            var applicationsCount = await _levyTransferMatchingService.GetApplicationsCount(accountId);

            var viewModel = new OrchestratorResponse<TransfersIndexViewModel>()
            {
                Data = new TransfersIndexViewModel()
                {
                    RenderCreateTransfersPledgeButton = renderCreateTransfersPledgeButton,
                    PledgesCount = pledgesCount,
                    ApplicationsCount = applicationsCount
                }
            };

            return viewModel;
        }
    }
}