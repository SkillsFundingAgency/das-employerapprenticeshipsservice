using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AgreementOrchestrator> _logger;
        private readonly IMapper _mapper;

        public AgreementOrchestrator(IMediator mediator, ILogger<AgreementOrchestrator> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<EmployerAgreementView> GetAgreement(long agreementId)
        {
            var response = await _mediator.Send(new GetEmployerAgreementByIdRequest
            {
                AgreementId = agreementId
            });

            return _mapper.Map<EmployerAgreementView>(response.EmployerAgreement);
        }

        public async Task<int> GetMinimumSignedAgreemmentVersion(long accountId)
        {
            _logger.LogInformation("Requesting minimum signed agreement version for account {AccountId}", accountId);

            var response = await _mediator.Send(new GetMinimumSignedAgreementVersionQuery { AccountId = accountId });
            return response.MinimumSignedAgreementVersion;
        }
    }
}