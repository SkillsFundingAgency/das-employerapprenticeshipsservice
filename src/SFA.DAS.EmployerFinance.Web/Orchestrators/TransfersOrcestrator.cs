using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class TransfersOrcestrator
    {
        private readonly IAuthorizationService _authorizationService;

        protected TransfersOrcestrator()
        {

        }

        public TransfersOrcestrator(
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<OrchestratorResponse<TransfersIndexViewModel>> Index()
        {
            bool renderCreateTransfersPledgeButton = await _authorizationService.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor);

            var viewModel = new OrchestratorResponse<TransfersIndexViewModel>()
            {
                Data = new TransfersIndexViewModel()
                {
                    RenderCreateTransfersPledgeButton = renderCreateTransfersPledgeButton,
                }
            };

            return viewModel;
        }
    }
}