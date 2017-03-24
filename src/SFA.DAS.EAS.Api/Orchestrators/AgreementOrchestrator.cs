using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Api.Orchestrators
{
    public class AgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public AgreementOrchestrator(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        public async Task<OrchestratorResponse<EmployerAgreementView>> GetAgreement(string hashedAgreementId)
        {
            var response = await _mediator.SendAsync(new GetEmployerAgreementByIdRequest
            {
                HashedAgreementId = hashedAgreementId
            });

            return new OrchestratorResponse<EmployerAgreementView>
            {
                Data = response.EmployerAgreement
            };
        }
    }
}