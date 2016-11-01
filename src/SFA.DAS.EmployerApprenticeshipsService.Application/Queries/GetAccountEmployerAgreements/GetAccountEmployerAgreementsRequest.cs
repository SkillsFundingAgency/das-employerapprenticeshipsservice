using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsRequest : IAsyncRequest<GetAccountEmployerAgreementsResponse>
    {
        public string HashedId { get; set; }
        public string ExternalUserId { get; set; }
    }
}