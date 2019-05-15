using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Organisation
    {
        [JsonConstructor]
        public Organisation()
        {
            Providers = new List<Provider>();
            ReserveFundings = new List<ReserveFunding>();
            Cohorts = new List<Cohort>();
            Agreements = new List<Agreement>();
        }
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("providers")]
        public ICollection<Provider> Providers { get; set; }
        [JsonProperty("reserveFundings")]
        public ICollection<ReserveFunding> ReserveFundings { get; set; }
        [JsonProperty("cohorts")]
        public ICollection<Cohort> Cohorts { get; set; }
        [JsonProperty("agreements")]
        public ICollection<Agreement> Agreements { get; set; }
        
    }
}