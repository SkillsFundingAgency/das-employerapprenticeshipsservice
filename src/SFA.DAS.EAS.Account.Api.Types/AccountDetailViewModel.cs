using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountDetailViewModel : IAccountResource
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string DasAccountId => HashedAccountId;
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OwnerEmail { get; set; }
        public ResourceList LegalEntities { get; set; }
        public ResourceList PayeSchemes { get; set; }
    }
}