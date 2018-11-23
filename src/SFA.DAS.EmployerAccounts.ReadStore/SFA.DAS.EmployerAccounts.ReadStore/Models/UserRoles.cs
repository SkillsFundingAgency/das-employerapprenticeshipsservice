using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    internal class UserRoles : Document
    {
        [JsonProperty("userRef")] public Guid UserRef { get; protected set; }

        [JsonProperty("accountId")] public long AccountId { get; protected set; }

        [JsonProperty("roles")] public IEnumerable<UserRole> Roles { get; protected set; } = new HashSet<UserRole>();

        [JsonProperty("outboxData")]
        public IEnumerable<OutboxMessage> OutboxData => _outboxData;

        [JsonProperty("updated")]
        public DateTime Updated { get; protected set; }

        [JsonProperty("deleted")]
        public DateTime? Deleted { get; protected set; }

        [JsonIgnore]
        private readonly List<OutboxMessage> _outboxData = new List<OutboxMessage>();

        public UserRoles(Guid userRef, long accountId, HashSet<UserRole> roles, string messageId, DateTime created) : base(1, "userRoles")
        {
            UserRef = userRef;
            AccountId = accountId;
            Roles = roles;
            Updated = created;
            AddMessageToOutbox(messageId, created);
        }

        [JsonConstructor]
        protected UserRoles()
        {
        }

        public void UpdateRoles(HashSet<UserRole> roles, DateTime updated, string messageId)
        {
            ProcessMessage(messageId, updated,
                () =>
                {
                    Roles = roles;
                    Updated = updated;
                    Deleted = null;
                }
            );
        }

        private void ProcessMessage(string messageId, DateTime messageCreated, Action action)
        {
            if (MessageAlreadyProcessed(messageId))
                return;

            AddMessageToOutbox(messageId, messageCreated);
            if (!IsMessageChronological(messageCreated))
            {
                return;
            }
            action();
        }

        private bool IsMessageChronological(DateTime messageDateTime)
        {
            var deleted = Deleted ?? DateTime.MinValue;

            return messageDateTime > Updated && messageDateTime > deleted;
        }

        private bool MessageAlreadyProcessed(string messageId)
        {
            return OutboxData.Any(x => x.MessageId == messageId);
        }

        private void AddMessageToOutbox(string messageId, DateTime created)
        {
            if (messageId is null) throw new ArgumentNullException(nameof(messageId));
            _outboxData.Add(new OutboxMessage(messageId, created));
        }
    }
}