using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Web.Models
{
    public class PayeSchemeDetail
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
        public string EmpRef { get; set; }
        public DateTime EmpRefAdded { get; set; }
    }
}