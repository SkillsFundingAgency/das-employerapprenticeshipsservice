using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class FrameworksView
    {
        public DateTime CreatedDate { get; set; }
        public List<Framework> Frameworks { get; set; }
    }
}