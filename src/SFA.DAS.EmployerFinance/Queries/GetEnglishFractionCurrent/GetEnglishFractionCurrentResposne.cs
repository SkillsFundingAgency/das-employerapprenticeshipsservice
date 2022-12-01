using System.Collections.Generic;
using SFA.DAS.EmployerFinance.Models.Levy;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFractionCurrent
{
    public class GetEnglishFractionCurrentResponse
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
    }
}