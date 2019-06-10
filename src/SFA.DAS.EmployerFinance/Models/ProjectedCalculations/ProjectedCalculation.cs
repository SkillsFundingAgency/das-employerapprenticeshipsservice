using System;

namespace SFA.DAS.EmployerFinance.Models.ProjectedCalculations
{
    public class ProjectedCalculation
    {
        public long AccountId { get; set; }
        public DateTime ProjectionGenerationDate { get; set; }
        public int NumberOfMonths { get; set; }
        public decimal FundsIn { get; set; }
        public decimal FundsOut { get; set; }
    }
}
