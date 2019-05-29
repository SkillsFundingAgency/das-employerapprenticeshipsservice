using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Database.Models
{
    public interface IOutbox
    {
        IEnumerable<OutboxMessage> OutboxData { get; }
        void DeleteOldMessages();
        void AddOutboxMessage(string messageId, DateTime created);
        bool IsMessageProcessed(string messageId);
    }
}
