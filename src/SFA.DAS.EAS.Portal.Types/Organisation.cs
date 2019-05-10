using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Organisation
    {
        public Organisation()
        {
            Providers = new List<Provider>();
            ReserveFundings = new List<ReserveFunding>();
            Cohorts = new List<Cohort>();
            Agreements = new List<Agreements>();
        }
        public long Id { get; set; }
        public ICollection<Provider> Providers { get; set; }
        public ICollection<ReserveFunding> ReserveFundings { get; set; }
        public ICollection<Cohort> Cohorts { get; set; }
        public ICollection<Agreements> Agreements { get; set; }
        
    }
}