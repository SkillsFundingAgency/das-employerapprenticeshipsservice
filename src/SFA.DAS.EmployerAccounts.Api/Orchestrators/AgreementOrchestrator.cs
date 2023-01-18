using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;
using SFA.DAS.EmployerAccounts.Queries.GetMinimumSignedAgreementVersion;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;

        public AgreementOrchestrator(IMediator mediator, ILog logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<EmployerAgreementView> GetAgreement(string hashedAgreementId)
        {
            var response = await _mediator.Send(new GetEmployerAgreementByIdRequest
            {
                HashedAgreementId = hashedAgreementId
            });

            return
                _mapper.Map<EmployerAgreementView>(response.EmployerAgreement);
        }

        public async Task<int> GetMinimumSignedAgreemmentVersion(long accountId)
        {
            _logger.Info($"Requesting minimum signed agreement version for account {accountId}");

            var response = await _mediator.Send(new GetMinimumSignedAgreementVersionQuery { AccountId = accountId });
            return response.MinimumSignedAgreementVersion;
        }
    }
}