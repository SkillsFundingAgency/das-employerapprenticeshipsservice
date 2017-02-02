using System;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Entities.Account;

namespace SFA.DAS.EAS.Application.Commands.CreateLegalEntity
{
    public class CreateLegalEntityCommand : IAsyncRequest<CreateLegalEntityCommandResponse>
    {
        public string HashedAccountId { get; set; }
        
        public LegalEntity LegalEntity { get; set; }

        public bool SignAgreement { get; set; }

        public DateTime SignedDate { get; set; }

        public string ExternalUserId { get; set; }
    }
}
