﻿namespace SFA.DAS.EAS.Domain.Models.Account
{
    public class AccountStats
    {
        public long AccountId { get; set; }
        public int PayeSchemeCount { get; set; }
        public int OrganisationCount { get; set; }
        public int TeamMemberCount { get; set; }
    }
}
