using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Account
    {
        [JsonConstructor]
        public Account()
        {
            Organisations = new List<Organisation>();
            PayeSchemes = new List<PayeScheme>();
        }

        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("organisations")]
        public ICollection<Organisation> Organisations { get; set; }
        [JsonProperty("deleted")]
        public DateTime? Deleted { get; set; }
        [JsonProperty("payeSchemes")]
        public ICollection<PayeScheme> PayeSchemes { get; set; }
    }
}
