using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using SFA.DAS.EAS.DasRecruitService.Models;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Account
    {
        public Account()
        {
            Organisations = new List<Organisation>();
            Providers = new List<Provider>();
            Vacancies = new List<Vacancy>();
        }

        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("organisations")]
        public ICollection<Organisation> Organisations { get; set; }
        [JsonProperty("providers")]
        public ICollection<Provider> Providers { get; set; }
        [JsonIgnore]
        public ICollection<Vacancy> Vacancies { get; set; }
        [JsonProperty("deleted")]
        public DateTime? Deleted { get; set; }
    }
}
