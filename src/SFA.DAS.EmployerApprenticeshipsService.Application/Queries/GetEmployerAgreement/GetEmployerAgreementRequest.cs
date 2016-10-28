using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementRequest : IAsyncRequest<GetEmployerAgreementResponse>
    {
        public string HashedId { get; set; }

        public string ExternalUserId { get; set; }
        public string HashedAgreementId { get; set; }
    }
}