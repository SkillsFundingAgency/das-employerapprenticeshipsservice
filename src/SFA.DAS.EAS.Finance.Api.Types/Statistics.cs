using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class Statistics
    {
        public long TotalAccounts { get; set; }
        public long TotalLegalEntities { get; set; }
        public long TotalPayeSchemes { get; set; }
        public long TotalAgreements { get; set; }
    }
}
