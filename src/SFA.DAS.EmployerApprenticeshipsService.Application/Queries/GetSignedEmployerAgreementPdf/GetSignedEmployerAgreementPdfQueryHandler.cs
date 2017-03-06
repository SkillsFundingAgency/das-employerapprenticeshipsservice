using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf
{
    public class GetSignedEmployerAgreementPdfQueryHandler : IAsyncRequestHandler<GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>
    {
        
        private readonly IValidator<GetSignedEmployerAgreementPdfRequest> _validator;
        private readonly IPdfService _pdfService;

        public GetSignedEmployerAgreementPdfQueryHandler(IValidator<GetSignedEmployerAgreementPdfRequest> validator, IPdfService pdfService)
        {
            _validator = validator;
            _pdfService = pdfService;
        }


        public async Task<GetSignedEmployerAgreementPdfResponse> Handle(GetSignedEmployerAgreementPdfRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }



            return new GetSignedEmployerAgreementPdfResponse();
        }
    }
}
