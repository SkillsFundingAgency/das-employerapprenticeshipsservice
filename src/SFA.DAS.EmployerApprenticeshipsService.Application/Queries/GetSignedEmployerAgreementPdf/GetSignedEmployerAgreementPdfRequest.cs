using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf
{
    public class GetSignedEmployerAgreementPdfRequest : IAsyncRequest<GetSignedEmployerAgreementPdfResponse>
    {
        public string HashedAccountId { get; set; }
        public string UserId { get; set; }
        public string HashedLegalEntityId { get; set; }
    }
}