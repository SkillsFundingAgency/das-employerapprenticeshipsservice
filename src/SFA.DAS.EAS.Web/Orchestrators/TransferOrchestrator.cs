using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using SFA.DAS.NLog.Logger;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class TransferOrchestrator
    {
        private readonly ILog _logger;
        private readonly IMediator _mediator;

        protected TransferOrchestrator()
        {

        }

        public TransferOrchestrator(IMediator mediator, ILog logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<TransferDashboardViewModel>> GetTransferAllowance(string hashedId, string externalUserId)
        {
            try
            {
                var transferBalanceResponse =
                    await _mediator.SendAsync(new GetTransferAllowanceRequest
                    {
                        HashedAccountId = hashedId,
                        ExternalUserId = externalUserId
                    });

                return new OrchestratorResponse<TransferDashboardViewModel>
                {
                    Data = new TransferDashboardViewModel { TransferAllowance = transferBalanceResponse.Balance },
                    Status = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to get transfer allowance for transfewr dashboard.");

                throw;
            }
        }
    }
}