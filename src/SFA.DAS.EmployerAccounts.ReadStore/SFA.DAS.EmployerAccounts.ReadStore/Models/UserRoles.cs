using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    internal class UserRoles : Document
    {
        [JsonProperty("userRef")]
        public Guid UserRef { get; protected set; }

        public long UserId { get; protected set; }

        [JsonProperty("accountId")]
        public long AccountId { get; protected set; }

        [JsonProperty("roles")]
        public IEnumerable<UserRole> Roles { get; protected set; } = new HashSet<UserRole>();

        [JsonProperty("outboxData")]
        public IEnumerable<OutboxMessage> OutboxData => _outboxData;

        [JsonProperty("Created")]
        public DateTime Created { get; protected set; }

        [JsonProperty("updated")]
        public DateTime? Updated { get; protected set; }

        [JsonProperty("removed")]
        public DateTime? Removed { get; protected set; }

        [JsonIgnore]
        private readonly List<OutboxMessage> _outboxData = new List<OutboxMessage>();

        public UserRoles(Guid userRef, long userId, long accountId, HashSet<UserRole> roles, string messageId, DateTime created) : base(1, "userRoles")
        {
            UserRef = userRef;
            UserId = userId;
            AccountId = accountId;
            Roles = roles;
            Created = created;
            AddMessageToOutbox(messageId, created);
        }

        [JsonConstructor]
        protected UserRoles()
        {
        }

        public void UpdateRoles(long userId, HashSet<UserRole> roles, DateTime updated, string messageId)
        {
            ProcessMessage(messageId, updated,
                () =>
                {
                    UserId = userId;
                    Roles = roles;
                    Updated = updated;
                    Removed = null;
                }
            );
        }

        public void Remove(DateTime deleted, string messageId)
        {
            ProcessMessage(messageId, deleted,
                () =>
                {
                    EnsureUserHasNotBeenRemoved();

                    Roles = new List<UserRole>();
                    Removed = deleted;
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
            return messageDateTime > Created && (Updated == null || messageDateTime > Updated.Value) && 
                   (Removed == null || messageDateTime > Removed.Value);

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

        private void EnsureUserHasNotBeenRemoved()
        {
            if (Removed != null)
            {
                throw new InvalidOperationException("User has already been removed");
            }
        }

    }
}