using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountDetailViewModel : IAccountResource
    {
        public string DasAccountId { get; set; }
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OwnerEmail { get; set; }
        public ResourceList LegalEntities { get; set; }
        public ResourceList PayeSchemes { get; set; }
    }
}