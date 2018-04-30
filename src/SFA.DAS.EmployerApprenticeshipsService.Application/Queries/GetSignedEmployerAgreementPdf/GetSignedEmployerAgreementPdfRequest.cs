﻿using System;
using MediatR;

namespace SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf
{
    public class GetSignedEmployerAgreementPdfRequest : IAsyncRequest<GetSignedEmployerAgreementPdfResponse>
    {
        public string HashedAccountId { get; set; }
        public Guid ExternalUserId { get; set; }
        public string HashedLegalAgreementId { get; set; }
    }
}