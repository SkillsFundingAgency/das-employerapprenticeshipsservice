﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Commands.SignEmployerAgreement
{
    public class SignEmployerAgreementCommand : IAsyncRequest
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public DateTime SignedDate { get; set; }
        public string HashedAgreementId { get; set; }

        public string OrganisationName { get; set; }
    }
}