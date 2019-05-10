using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Cohort
    {
        public Cohort()
        {
            Apprenticeships = new List<Apprenticeship>();
        }
        public string Id { get; set; }
        public ICollection<Apprenticeship> Apprenticeships { get; set; }
    }
}