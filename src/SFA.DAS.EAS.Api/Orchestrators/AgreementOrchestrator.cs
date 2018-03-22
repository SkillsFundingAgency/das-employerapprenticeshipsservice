using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Api.Orchestrators
{
    public class AgreementOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AgreementOrchestrator(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        public async Task<OrchestratorResponse<EmployerAgreementView>> GetAgreement(string hashedAgreementId)
        {
            var response = await _mediator.SendAsync(new GetEmployerAgreementByIdRequest
            {
                HashedAgreementId = hashedAgreementId
            });

            return new OrchestratorResponse<EmployerAgreementView>
            {
                Data = _mapper.Map<EmployerAgreementView>(response.EmployerAgreement)
            };
        }
    }
}