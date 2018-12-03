using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse
{
    public class StandardsView
    {
        public DateTime CreationDate { get; set; }
        public List<Standard> Standards { get; set; }
    }
}