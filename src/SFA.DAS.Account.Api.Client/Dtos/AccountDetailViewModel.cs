using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Account.Api.Client.Dtos
{
    public class AccountDetailViewModel
    {
        public string DasAccountId { get; set; }
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OwnerEmail { get; set; }
        public List<ResourceViewModel> LegalEntities { get; set; }
        public List<ResourceViewModel> PayeSchemes { get; set; }
    }
}