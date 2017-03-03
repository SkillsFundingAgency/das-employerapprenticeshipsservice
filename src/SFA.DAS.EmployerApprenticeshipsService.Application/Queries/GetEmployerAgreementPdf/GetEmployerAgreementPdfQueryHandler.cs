using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfQueryHandler : IAsyncRequestHandler<GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
    {
        private readonly IValidator<GetEmployerAgreementPdfRequest> _validator;
        private readonly IPdfService _pdfService;

        public GetEmployerAgreementPdfQueryHandler(IValidator<GetEmployerAgreementPdfRequest> validator, IPdfService pdfService)
        {
            _validator = validator;
            _pdfService = pdfService;
        }

        public async Task<GetEmployerAgreementPdfResponse> Handle(GetEmployerAgreementPdfRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var file = await _pdfService.SubsituteValuesForPdf(message.AgreementFileName);


            return new GetEmployerAgreementPdfResponse {FileStream = file};
        }
    }
}
