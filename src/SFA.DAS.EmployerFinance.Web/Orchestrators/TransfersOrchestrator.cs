using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerFinance.Web.ViewModels.Transfers;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class TransfersOrchestrator
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHashingService _hashingService;
        private readonly IManageApprenticeshipsService _manageApprenticeshipsService;

        protected TransfersOrchestrator()
        {

        }

        public TransfersOrchestrator(
            IAuthorizationService authorizationService,
            IHashingService hashingService,
            IManageApprenticeshipsService manageApprenticeshipsService)
        {
            _authorizationService = authorizationService;
            _hashingService = hashingService;
            _manageApprenticeshipsService = manageApprenticeshipsService;
        }

        public async Task<OrchestratorResponse<IndexViewModel>> GetIndexViewModel(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var indexTask = _manageApprenticeshipsService.GetIndex(accountId);
            var renderCreateTransfersPledgeButtonTask = _authorizationService.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor);

            await Task.WhenAll(indexTask, renderCreateTransfersPledgeButtonTask);

            return new OrchestratorResponse<IndexViewModel>
            {
                Data = new IndexViewModel
                {
                    PledgesCount = indexTask.Result.PledgesCount,
                    IsTransferReceiver = indexTask.Result.IsTransferReceiver
                }
            };
        }
    }
}