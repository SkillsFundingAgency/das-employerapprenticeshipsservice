using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf
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

            if (legalAgreement.Status == EmployerAgreementStatus.Pending || legalAgreement.Status == EmployerAgreementStatus.Removed || !legalAgreement.SignedDate.HasValue)
            {
                throw new InvalidRequestException(new Dictionary<string, string> { { nameof(legalAgreement.Status), "The agreement has not been signed." } });
            }

            var substituteValues = new Dictionary<string, string>
            {
                { nameof(legalAgreement.SignedByName), legalAgreement.SignedByName },
                { nameof(legalAgreement.SignedDate), legalAgreement.SignedDate.Value.ToString("d MMMM yyyy") },
                { nameof(legalAgreement.LegalEntityName), legalAgreement.LegalEntityName }
            };

            var addressElements = Enumerable.ToList<string>(legalAgreement.LegalEntityAddress.Split(','));
            
            for (var i = 0; i < 5; i++)
            {
                var addressLine = "";
                if (addressElements.Count > i)
                {
                    addressLine= addressElements[i];
                }
                
                substituteValues.Add($"{nameof(legalAgreement.LegalEntityAddress)}_{i}", addressLine.Trim());
            }
            
            var pdfStream = await _pdfService.SubsituteValuesForPdf($"{legalAgreement.TemplatePartialViewName}_Sub.pdf", substituteValues);

            return new GetSignedEmployerAgreementPdfResponse { FileStream = pdfStream };
        }
    }
}
