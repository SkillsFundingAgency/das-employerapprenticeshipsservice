using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove
{
    public class GetAccountEmployerAgreementRemoveRequest : IAsyncRequest<GetAccountEmployerAgreementRemoveResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public string HashedAgreementId { get; set; }
    }
}