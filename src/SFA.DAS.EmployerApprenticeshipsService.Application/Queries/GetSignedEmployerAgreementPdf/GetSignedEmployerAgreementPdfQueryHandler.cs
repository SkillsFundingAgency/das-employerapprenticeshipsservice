using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf
{
    public class GetSignedEmployerAgreementPdfQueryHandler : IAsyncRequestHandler<GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>
    {
        
        private readonly IValidator<GetSignedEmployerAgreementPdfRequest> _validator;
        private readonly IPdfService _pdfService;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;

        public GetSignedEmployerAgreementPdfQueryHandler(IValidator<GetSignedEmployerAgreementPdfRequest> validator, IPdfService pdfService, IEmployerAgreementRepository employerAgreementRepository, IHashingService hashingService)
        {
            _validator = validator;
            _pdfService = pdfService;
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
        }


        public async Task<GetSignedEmployerAgreementPdfResponse> Handle(GetSignedEmployerAgreementPdfRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var legalAgreementId = _hashingService.DecodeValue(message.HashedLegalAgreementId);

            var legalAgreement = await _employerAgreementRepository.GetEmployerAgreement(legalAgreementId);

            if (legalAgreement.Status != EmployerAgreementStatus.Signed || !legalAgreement.SignedDate.HasValue)
            {
                throw new InvalidRequestException(new Dictionary<string, string> {{nameof(legalAgreement.Status), "The agreement has not been signed."}});
            }

            var substituteValues = new Dictionary<string, string>
            {
                {nameof(legalAgreement.SignedByName), legalAgreement.SignedByName},
                {nameof(legalAgreement.SignedDate), legalAgreement.SignedDate.Value.ToLongDateString()},
                {nameof(legalAgreement.LegalEntityAddress), legalAgreement.LegalEntityAddress},
                {nameof(legalAgreement.LegalEntityName), legalAgreement.LegalEntityName}
            };


            var pdfStream = await _pdfService.SubsituteValuesForPdf($"{legalAgreement.TemplatePartialViewName}_Sub.pdf", substituteValues);

            return new GetSignedEmployerAgreementPdfResponse {FileStream = pdfStream };
        }
    }
}
