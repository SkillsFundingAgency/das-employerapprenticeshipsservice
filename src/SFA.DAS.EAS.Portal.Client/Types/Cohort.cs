using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Cohort
    {
        [JsonConstructor]
        public Cohort()
        {
            Apprenticeships = new List<Apprenticeship>();
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("apprenticeships")]
        public ICollection<Apprenticeship> Apprenticeships { get; set; }
    }
}