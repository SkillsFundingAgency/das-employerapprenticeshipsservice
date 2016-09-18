using MediatR;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLegalEntityAgreement
{
    public class GetLegalEntityAgreementRequest : IAsyncRequest<GetLegalEntityAgreementResponse>
    {
        public long AccountId { get; set; }
        public string LegalEntityCode { get; set; }
    }
}
