using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Models.Features
{
    public enum FeatureType
    {
        NotSpecified,
        Activities,
        Projections,
        Recruitments,
        TransferConnectionRequests,
        Transfers,

        // These enums are only used in unit tests - the numbers can be changed 
        Test1 = 100,
        Test2 = 101
    }
}
