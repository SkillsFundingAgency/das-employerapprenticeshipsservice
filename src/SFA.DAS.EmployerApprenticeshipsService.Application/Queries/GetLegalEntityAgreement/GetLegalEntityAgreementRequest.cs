using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement
{
    public class GetLegalEntityAgreementRequest : IAsyncRequest<GetLegalEntityAgreementResponse>
    {
        public long AccountId { get; set; }
        public string LegalEntityCode { get; set; }
    }
}
