using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestAccountAgreementTemplate
{
    public class GetLatestAccountAgreementTemplateQueryHandler : 
        IAsyncRequestHandler<GetLatestAccountAgreementTemplateRequest, GetLatestAccountAgreementResponse>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public GetLatestAccountAgreementTemplateQueryHandler(
            IEmployerAgreementRepository employerAgreementRepository)
        {
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<GetLatestAccountAgreementResponse> Handle(GetLatestAccountAgreementTemplateRequest message)
        {
            var template = await _employerAgreementRepository.GetLatestAgreementTemplate();

            return new GetLatestAccountAgreementResponse { Template = template};
        }
    }
}
