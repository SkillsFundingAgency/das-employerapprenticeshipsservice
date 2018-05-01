﻿using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreement
{
    public class GetEmployerAgreementRequest : IAsyncRequest<GetEmployerAgreementResponse>
    {
        public string HashedAccountId { get; set; }

        public string AgreementId { get; set; }

        public string ExternalUserId { get; set; }
    }
}