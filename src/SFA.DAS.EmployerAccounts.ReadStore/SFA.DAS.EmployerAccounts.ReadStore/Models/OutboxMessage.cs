using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.ReadStore.Models
{
    public class OutboxMessage
    {
        [JsonProperty("messageId")]
        public string MessageId { get; protected set; }

        [JsonProperty("created")]
        public DateTime Created { get; protected set; }

        public OutboxMessage(string messageId, DateTime created)
        {
            MessageId = messageId;
            Created = created;
        }
    }
}
