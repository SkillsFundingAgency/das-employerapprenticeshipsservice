using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    public class UserRoles : Document
    {
        [JsonProperty("userRef")] public Guid UserRef { get; protected set; }

        [JsonProperty("accountId")] public long AccountId { get; protected set; }

        [JsonProperty("roles")] public IEnumerable<short> Roles { get; protected set; } = new HashSet<short>();


        [JsonProperty("created")] public DateTime Created { get; protected set; }

        [JsonProperty("updated")] public DateTime? Updated { get; protected set; }

        public UserRoles(Guid userRef, long accountId, HashSet<short> roles, DateTime created) : base(1, "userRoles")
        {
            UserRef = userRef;
            AccountId = accountId;
            Roles = roles;
            Created = created;
        }

        [JsonConstructor]
        protected UserRoles()
        {
        }
    }
}