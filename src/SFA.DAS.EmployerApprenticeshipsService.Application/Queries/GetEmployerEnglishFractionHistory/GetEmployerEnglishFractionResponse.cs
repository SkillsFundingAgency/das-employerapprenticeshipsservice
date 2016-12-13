using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerEnglishFractionHistory
{
    public class GetEmployerEnglishFractionResponse
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
        public string EmpRef { get; set; }
        public DateTime EmpRefAddedDate { get; set; }
    }
}