using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf
{
    public class GetEmployerAgreementPdfRequest : IAsyncRequest<GetEmployerAgreementPdfResponse>
    {
        public string HashedAccountId { get; set; }
        public string HashedLegalAgreementId { get; set; }
        public string UserId { get; set; }
    }
}