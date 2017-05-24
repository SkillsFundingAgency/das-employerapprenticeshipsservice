using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Models.EmployerAgreement
{
    public class RemoveEmployerAgreementView
    {
        public string Name { get; set; }

        public bool CanBeRemoved { get; set; }
    }
}