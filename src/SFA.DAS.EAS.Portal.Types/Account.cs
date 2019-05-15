using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Account
    {
        [JsonConstructor]
        public Account()
        {
            Organisations = new List<Organisation>();
            SavedStandards = new List<object>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("organisations")]
        public ICollection<Organisation> Organisations { get; set; }
        [JsonProperty("savedStandards")]
        public ICollection<object> SavedStandards { get; set; }
        [JsonProperty("deleted")]
        public DateTime? Deleted { get; set; }
    }
}
