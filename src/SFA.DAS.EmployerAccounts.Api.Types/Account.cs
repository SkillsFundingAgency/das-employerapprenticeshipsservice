﻿using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class Account
    {
        public string AccountName { get; set; }
        public string AccountHashId { get; set; }
        public string PublicAccountHashId { get; set; }
        public long AccountId { get; set; }
        public string Href { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public AccountAgreementType AccountAgreementType { get; set; }
    }
}
