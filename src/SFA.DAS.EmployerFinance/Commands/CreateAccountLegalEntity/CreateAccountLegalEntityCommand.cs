using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SFA.DAS.EmployerFinance.Commands.CreateAccountLegalEntity
{
    public class CreateAccountLegalEntityCommand : IAsyncRequest
    {
        public CreateAccountLegalEntityCommand(long id, DateTime deleted, long pendingAgreementId, long signedAgreementId, int signedAgreementVersion, long accountId, long legalEntityId)
        {
            Id = id;
            Deleted = deleted;
            PendingAgreementId = pendingAgreementId;
            SignedAgreementId = signedAgreementId;
            SignedAgreementVersion = signedAgreementVersion;
            AccountId = accountId;
            LegalEntityId = legalEntityId;
        }

        public long Id { get; set; }
        public DateTime Deleted { get; set; }
        public long PendingAgreementId { get; set; }
        public long SignedAgreementId { get; set; }
        public int SignedAgreementVersion { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}
