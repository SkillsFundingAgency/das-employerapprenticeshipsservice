using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
/* Legal Entity Array
	AccountLegalEntityId (and/or LegalEntityId), Name
	Funding Reservation Array
		Reservation ID
		Course Name (optional)
		Start Date
		End Date
		Deleted? (don't worry too much for mvp?)
 */
    
    
    public class Account : Document //todo:, IAccountDto
    {
        //todo: doc id / accountid
        [JsonProperty("accountId")]
        public long AccountId { get; private set; }

        [JsonProperty("accountLegalEntities")]
        public IEnumerable<AccountLegalEntity> AccountLegalEntities => _accountLegalEntities;

        [JsonProperty("outboxData")]
        public IEnumerable<OutboxMessage> OutboxData => _outboxData;
        
        [JsonProperty("created")]
        public DateTime Created { get; private set; }

        [JsonProperty("updated")]
        public DateTime? Updated { get; private set; }

        [JsonProperty("deleted")]
        public DateTime? Deleted { get; private set; }

        [JsonIgnore]
        private readonly List<AccountLegalEntity> _accountLegalEntities = new List<AccountLegalEntity>();

        [JsonIgnore]
        private readonly List<OutboxMessage> _outboxData = new List<OutboxMessage>();

        public Account(long accountId, DateTime created, string messageId)
            : base(1)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            Created = created;

            AddOutboxMessage(messageId, created);
        }

        [JsonConstructor]
        private Account()
        {
        }

        public void Delete(DateTime deleted, string messageId)
        {
            //todo: nested class?
            ProcessMessage(messageId, deleted, () =>
            {
                EnsureHasNotBeenDeleted();

                Deleted = deleted;
            });
        }

        private void AddOutboxMessage(string messageId, DateTime created)
        {
            if (messageId == null)
                throw new ArgumentNullException(nameof(messageId));
            
            _outboxData.Add(new OutboxMessage(messageId, created));
        }

        private void EnsureHasNotBeenDeleted()
        {
            if (Deleted != null)
                throw new InvalidOperationException("Requires account has not been deleted");
        }

        private bool IsMessageProcessed(string messageId)
        {
            return OutboxData.Any(m => m.MessageId == messageId);
        }

        private void ProcessMessage(string messageId, DateTime created, Action action)
        {
            if (!IsMessageProcessed(messageId))
            {
                action();
                AddOutboxMessage(messageId, created);
            }
        }
    }
}