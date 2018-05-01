using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove
{
    public class GetAccountEmployerAgreementsRemoveRequest : IAsyncRequest<GetAccountEmployerAgreementsRemoveResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
    }
}
