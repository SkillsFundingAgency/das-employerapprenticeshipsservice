using System;

namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class DasEnglishFraction
    {
        public string Id { get; set; }
        public DateTime DateCalculated { get; set; }
        public decimal Amount { get; set; }
        public string EmpRef { get; set; }
    }
}
