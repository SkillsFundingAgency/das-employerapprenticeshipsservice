using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Queries.GetLatestEmployerAgreementTemplate
{
    public class GetLatestEmployerAgreementTemplateQueryHandler : 
        IAsyncRequestHandler<GetLatestEmployerAgreementTemplateRequest, GetLatestEmployerAgreementResponse>
    {
        private readonly IValidator<GetLatestEmployerAgreementTemplateRequest> _validator;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public GetLatestEmployerAgreementTemplateQueryHandler(
            IValidator<GetLatestEmployerAgreementTemplateRequest> validator,
            IEmployerAgreementRepository employerAgreementRepository)
        {
            _validator = validator;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<GetLatestEmployerAgreementResponse> Handle(GetLatestEmployerAgreementTemplateRequest message)
        {
            var validationResults = await _validator.ValidateAsync(message);

            if (validationResults.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }

            var template = await _employerAgreementRepository.GetLatestAgreementTemplate();

            return new GetLatestEmployerAgreementResponse { Template = template};
        }
    }
}
