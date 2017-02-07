using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider
{
    public class ProvidersView
    {
        public DateTime CreatedDate { get; set; }
        public List<Provider> Providers { get; set; }
    }
}