using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementRequest : IAsyncRequest<GetEmployerAgreementResponse>
    {
        public string HashedAccountId { get; set; }

        public Guid ExternalUserId { get; set; }
        public string HashedAgreementId { get; set; }
    }
}