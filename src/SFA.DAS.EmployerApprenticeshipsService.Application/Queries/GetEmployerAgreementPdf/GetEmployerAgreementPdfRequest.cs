using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfRequest : IAsyncRequest<GetEmployerAgreementPdfResponse>
    {
        public string AgreementFileName { get; set; }
    }
}