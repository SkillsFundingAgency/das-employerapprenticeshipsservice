using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommand : IAsyncRequest
    {
        public SignEmployerAgreementCommand(string hashedAccountId, string externalUserId, DateTime signedDate, string hashedAgreementId, string companyName)
        {
            HashedAccountId = hashedAccountId;
            ExternalUserId = externalUserId;
            SignedDate = signedDate;
            HashedAgreementId = hashedAgreementId;
        }

        public string HashedAccountId { get;  }
        public string ExternalUserId { get;  }
        public DateTime SignedDate { get;  }
        public string HashedAgreementId { get;  }

        public string CompanyName { get; } = "todo";
    }
}