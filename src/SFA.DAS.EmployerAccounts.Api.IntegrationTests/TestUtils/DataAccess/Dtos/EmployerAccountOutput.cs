﻿namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class EmployerAccountOutput
    {
        public long AccountId { get; set; }
        public string? HashedAccountId { get; set; }
        public string? PublicHashedAccountId { get; set; }
        public long LegalEntityId { get; set; }
    }
}