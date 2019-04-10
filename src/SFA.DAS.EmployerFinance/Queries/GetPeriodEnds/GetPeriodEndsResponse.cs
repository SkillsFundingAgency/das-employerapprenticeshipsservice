
using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Payments;

namespace SFA.DAS.EmployerFinance.Queries.GetPeriodEnds
{
    public class GetPeriodEndsResponse
    {
        public List<PeriodEnd> CurrentPeriodEnds { get; set; }
    }
}