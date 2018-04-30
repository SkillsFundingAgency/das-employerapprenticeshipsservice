using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.RemoveLegalEntity
{
    public class RemoveLegalEntityCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public string HashedLegalAgreementId { get; set; }
    }
}
