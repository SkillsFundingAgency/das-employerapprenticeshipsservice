using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfQueryHandler : IAsyncRequestHandler<GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
    {
        private readonly IValidator<GetEmployerAgreementPdfRequest> _validator;
        private readonly IPdfService _pdfService;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetEmployerAgreementPdfQueryHandler(IValidator<GetEmployerAgreementPdfRequest> validator, IPdfService pdfService, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _validator = validator;
            _pdfService = pdfService;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }

        public async Task<GetEmployerAgreementPdfResponse> Handle(GetEmployerAgreementPdfRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var employerAgreementId = _hashingService.DecodeValue(message.HashedLegalAgreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(employerAgreementId);

            var file = await _pdfService.SubsituteValuesForPdf($"{agreement.TemplatePartialViewName}.pdf");


            return new GetEmployerAgreementPdfResponse {FileStream = file};
        }
    }
}
