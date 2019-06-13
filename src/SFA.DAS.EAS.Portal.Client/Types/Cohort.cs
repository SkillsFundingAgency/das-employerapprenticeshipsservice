using Newtonsoft.Json;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Cohort
    {
        public Cohort()
        {
            Apprenticeships = new List<Apprenticeship>();
        }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("reference")]
        public string Reference { get; set; }
        [JsonProperty("apprenticeships")]
        public ICollection<Apprenticeship> Apprenticeships { get; set; }
    }
}