using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsRequest : IAsyncRequest<GetAccountEmployerAgreementsResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}