using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class ProvidersView
    {
        public DateTime CreatedDate { get; set; }
        public List<Provider> Providers { get; set; }
    }
}