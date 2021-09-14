using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Apprenticeships;

namespace SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses
{
    public class GetApplicationsResponse
    {
        public IEnumerable<ApprenticeshipDetail> Apprenticeships { get; set; }
    }
}
