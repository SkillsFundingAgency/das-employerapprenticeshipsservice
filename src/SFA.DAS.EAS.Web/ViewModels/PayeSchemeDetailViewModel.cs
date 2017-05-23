using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class PayeSchemeDetailViewModel
    {
        public IEnumerable<DasEnglishFraction> Fractions { get; set; }
        public string EmpRef { get; set; }
        public string PayeSchemeName { get; set; }
        public DateTime EmpRefAdded { get; set; }
    }
}