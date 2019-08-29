using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
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
        
        public async Task<EmployerAgreementView> GetAgreement(string hashedAgreementId)
        {
            var response = await _mediator.SendAsync(new GetEmployerAgreementByIdRequest
            {
                HashedAgreementId = hashedAgreementId
            });

            return
                _mapper.Map<EmployerAgreementView>(response.EmployerAgreement);
        }
    }
}