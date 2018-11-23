using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    internal class UserRoles : Document
    {
        [JsonProperty("userRef")] public Guid UserRef { get; protected set; }

        [JsonProperty("accountId")] public long AccountId { get; protected set; }

        [JsonProperty("roles")] public IEnumerable<UserRole> Roles { get; protected set; } = new HashSet<UserRole>();

        [JsonProperty("created")] public DateTime Created { get; protected set; }

        [JsonProperty("updated")] public DateTime? Updated { get; protected set; }

        public UserRoles(Guid userRef, long accountId, HashSet<UserRole> roles, DateTime created) : base(1, "userRoles")
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