using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    internal class AccountUser : Document
    {
        [JsonProperty("userRef")]
        public Guid UserRef { get; protected set; }

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

        public AccountUser(Guid userRef, long accountId, HashSet<UserRole> roles, DateTime created, string messageId) : base(1, "userRoles")
        {
            UserRef = userRef;
            AccountId = accountId;
            Roles = roles;
            Created = created;
            AddOutboxMessage(messageId, created);
            Id = Guid.NewGuid();
        }

        [JsonConstructor]
        protected AccountUser()
        {
        }

        public void Recreate(HashSet<UserRole> roles, DateTime recreated, string messageId)
        {
            ProcessMessage(messageId, recreated,
                () =>
                {
                    if (IsRecreateDateChronological(recreated))
                    {
                        EnsureUserHasBeenRemoved();

                        Roles = roles;
                        Created = recreated;
                        Updated = null;
                        Removed = null;
                    }
                }
            );
        }

        public void UpdateRoles(HashSet<UserRole> roles, DateTime updated, string messageId)
        {
            ProcessMessage(messageId, updated,
                () =>
                {
                    if (IsUpdatedRolesDateChronological(updated))
                    {
                        EnsureUserHasNotBeenRemoved();

                        Roles = roles;
                        Updated = updated;
                    }
                }
            );
        }

        public void Remove(DateTime removed, string messageId)
        {
            ProcessMessage(messageId, removed,
                () =>
                {
                    if (IsRemovedDateChronological(removed))
                    {
                        EnsureUserHasNotBeenRemoved();

                        Roles = new List<UserRole>();
                        Removed = removed;
                    }
                }
            );
        }

        public bool HasRole(HashSet<UserRole> roles)
        {
            return Roles.Any(role => roles.Any(requestRole => requestRole == role));
        }

        private void ProcessMessage(string messageId, DateTime messageCreated, Action action)
        {
            if (IsMessageProcessed(messageId))
                return;

            action();
            AddOutboxMessage(messageId, messageCreated);
        }

        private bool IsRecreateDateChronological(DateTime recreate)
        {
            return recreate > Created;
        }

        private bool IsUpdatedRolesDateChronological(DateTime updated)
        {
            return updated > Created && (Updated == null || updated > Updated.Value) && (Removed == null || updated > Removed.Value);
        }

        private bool IsRemovedDateChronological(DateTime removed)
        {
            return removed > Created && (Removed == null || removed > Removed.Value);
        }

        private bool IsMessageProcessed(string messageId)
        {
            return OutboxData.Any(x => x.MessageId == messageId);
        }

        private void AddOutboxMessage(string messageId, DateTime created)
        {
            if (messageId is null)
            {
                throw new ArgumentNullException(nameof(messageId));
            }
            _outboxData.Add(new OutboxMessage(messageId, created));
        }

        private void EnsureUserHasBeenRemoved()
        {
            if (Removed == null)
            {
                throw new InvalidOperationException("User has not been removed");
            }
        }

        private void EnsureUserHasNotBeenRemoved()
        {
            if (Removed != null)
            {
                throw new InvalidOperationException("User has already been removed");
            }
        }

        private void EnsureUserHasNotBeenCreatedOrUpdatedAfterRemoveAction(DateTime removed)
        {
            if (!IsUpdatedRolesDateChronological(removed))
            {
                throw new InvalidOperationException("User has been reinvoked or updated after remove request");
            }
        }
    }
}