using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Levy;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionResponse
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
        public string EmpRef { get; set; }
        public DateTime EmpRefAddedDate { get; set; }
    }
}