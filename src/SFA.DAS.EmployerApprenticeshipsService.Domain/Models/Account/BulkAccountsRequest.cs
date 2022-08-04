using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    public class BulkAccountsRequest
    {
        public List<long> AccountIds { get; set; }
    }

}
