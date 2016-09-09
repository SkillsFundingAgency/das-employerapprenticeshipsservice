using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestEmployerAgreementTemplate
{
    public class GetLatestEmployerAgreementTemplateQueryHandler : 
        IAsyncRequestHandler<GetLatestEmployerAgreementTemplateRequest, GetLatestEmployerAgreementResponse>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public GetLatestEmployerAgreementTemplateQueryHandler(IEmployerAgreementRepository employerAgreementRepository)
        {
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<GetLatestEmployerAgreementResponse> Handle(GetLatestEmployerAgreementTemplateRequest message)
        {
            var template = await _employerAgreementRepository.GetLatestAgreementTemplate();

            return new GetLatestEmployerAgreementResponse { Template = template};
        }
    }
}
