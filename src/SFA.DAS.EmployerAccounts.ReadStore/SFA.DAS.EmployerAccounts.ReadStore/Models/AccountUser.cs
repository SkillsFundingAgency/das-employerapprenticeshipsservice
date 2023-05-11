using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models;

public class AccountUser : Document
{
    [JsonProperty("userRef")]
    public Guid UserRef { get; private set; }

    [JsonProperty("accountId")]
    public long AccountId { get; private set; }

    [JsonProperty("role")]
    public UserRole? Role { get; private set; }

    [JsonProperty("outboxData")]
    public IEnumerable<OutboxMessage> OutboxData => _outboxData;

    [JsonProperty("Created")]
    public DateTime Created { get; private set; }

    [JsonProperty("updated")]
    public DateTime? Updated { get; private set; }

    [JsonProperty("removed")]
    public DateTime? Removed { get; private set; }

    [JsonIgnore]
    private readonly List<OutboxMessage> _outboxData = new List<OutboxMessage>();

    public AccountUser(Guid userRef, long accountId, UserRole role, DateTime created, string messageId) : base(1, "accountUser")
    {
        UserRef = userRef;
        AccountId = accountId;
        Role = role;
        Created = created;
        AddOutboxMessage(messageId, created);
        Id = Guid.NewGuid();
    }

    [JsonConstructor]
    private AccountUser()
    {
    }

    public void Recreate(UserRole role, DateTime recreated, string messageId)
    {
        ProcessMessage(messageId, recreated,
            () =>
            {
                if (IsRecreateDateChronological(recreated))
                {
                    EnsureUserHasBeenRemoved();

                    Role = role;
                    Created = recreated;
                    Updated = null;
                    Removed = null;
                }
            }
        );
    }

    public void UpdateRoles(UserRole role, DateTime updated, string messageId)
    {
        ProcessMessage(messageId, updated,
            () =>
            {
                if (IsUpdatedRolesDateChronological(updated))
                {
                    EnsureUserHasNotBeenRemoved();

                    Role = role;
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

                    Role = null;
                    Removed = removed;
                }
            }
        );
    }

    private void ProcessMessage(string messageId, DateTime messageCreated, Action action)
    {
        if (IsMessageProcessed(messageId))
        {
            return;
        }

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
}