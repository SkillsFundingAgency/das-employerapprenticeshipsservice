using System;

namespace SFA.DAS.EmployerFinance.Queries.GetEnglishFractionsUpdateRequired
{
    public class GetEnglishFractionUpdateRequiredResponse
    {
        public bool UpdateRequired { get; set; }
        public DateTime DateCalculated { get; set; }
    }
}
