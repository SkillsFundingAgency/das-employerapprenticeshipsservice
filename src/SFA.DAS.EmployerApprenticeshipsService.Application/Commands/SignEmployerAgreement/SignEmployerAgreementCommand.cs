using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommand : IAsyncRequest
    {
        public SignEmployerAgreementCommand(string hashedAccountId, string externalUserId, DateTime signedDate, string hashedAgreementId, string organisationName)
        {
            HashedAccountId = hashedAccountId;
            ExternalUserId = externalUserId;
            SignedDate = signedDate;
            HashedAgreementId = hashedAgreementId;
            OrganisationName = organisationName;
        }

        public string HashedAccountId { get;  }
        public string ExternalUserId { get;  }
        public DateTime SignedDate { get;  }
        public string HashedAgreementId { get;  }

        public string OrganisationName { get; }
    }
}