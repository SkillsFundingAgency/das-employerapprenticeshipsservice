using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
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
        private List<OutboxMessage> _outboxData = new List<OutboxMessage>();

        private Account(long accountId, DateTime created, string messageId)
            : base(1)
        {
            Id = Guid.NewGuid();
            AccountId = accountId;
            // the created date originates from event publishers, so we make sure we store it as UTC
            Created = DateTime.SpecifyKind(created, DateTimeKind.Utc);
            
            AddOutboxMessage(messageId, created);
        }

        // ctor for when creating account doc from reserved funding
        // do we accept event, seems a coupling too far!
        public Account(long accountId, long accountLegalEntityId, string legalEntityName, long reservationId,
            long courseId, string courseName, DateTime startDate, DateTime endDate, DateTime created, string messageId)
            : this(accountId, created, messageId)
        {
            AddReserveFunding(accountLegalEntityId, legalEntityName, reservationId, courseId, courseName, startDate, endDate);
        }

        [JsonConstructor]
        private Account()
        {
        }

        public void AddReserveFunding(long accountLegalEntityId, string legalEntityName,  long reservationId,
            long courseId, string courseName, DateTime startDate, DateTime endDate, DateTime updated, string messageId)
        {
            ProcessMessage(messageId, updated, () =>
            {
                // don't need to check this when called as part of creation, but we separate out creation and adding reserve funding
                // as other events handlers will create the account and add their own specific data
                // we could have different ctors/factories for creation from different events
                // for now we include updated == created date to let through adding on creation
                // todo: need lots of tests around this out-of order handling!!
                if (IsUpdatedDateChronological(updated))
                {
                    // we also need to handle case where reserve funding is created, then deleted, then created again with same details
                    //EnsureRelationshipHasNotBeenDeleted();
                    
                    // enforce uniqueness with cosmos unique key?
                    var existingAccountLegalEntity = _accountLegalEntities.FirstOrDefault(ale => ale.AccountLegalEntityId == accountLegalEntityId);
                    if (existingAccountLegalEntity == null)
                        AddReserveFunding(accountLegalEntityId, legalEntityName, reservationId, courseId, courseName, startDate, endDate);
                    else
                        existingAccountLegalEntity.AddReserveFunding(reservationId, courseId, courseName, startDate, endDate);
                    
                    Updated = updated;
                    Deleted = null;
                }
            });
        }

        private void AddReserveFunding(long accountLegalEntityId, string legalEntityName,
            long reservationId, long courseId, string courseName, DateTime startDate, DateTime endDate)
        {
            _accountLegalEntities.Add(new AccountLegalEntity(accountLegalEntityId, legalEntityName, reservationId, courseId, courseName, startDate, endDate));
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

        private bool IsUpdatedDateChronological(DateTime updated)
        {
            return updated > Created && (Updated == null || updated > Updated.Value) && (Deleted == null || updated > Deleted.Value);
            //return updated >= Created && (Updated == null || updated > Updated.Value) && (Deleted == null || updated > Deleted.Value);
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

        // todo: it would be better to have a webjob to periodically clean-up, but for now..
        private void DeleteOldMessages()
        {
            //todo: configurable. think what we want to set this to         we need to ensure the expiry period is > total retry time! - no we don't, the message will only be there if the message was processed ok!
            var expiryPeriod = TimeSpan.Parse("2");
            //todo: unit testable
            var now = DateTime.UtcNow;
            _outboxData = _outboxData.Where(m => m.Created - now > expiryPeriod).ToList();
        }
        
        private bool IsMessageProcessed(string messageId)
        {
            return OutboxData.Any(m => m.MessageId == messageId);
        }

        private void ProcessMessage(string messageId, DateTime created, Action action)
        {
            DeleteOldMessages();
            
            if (IsMessageProcessed(messageId))
                return;
            
            action();
            AddOutboxMessage(messageId, created);
        }

//        private abstract class MessageProcessor
//        {
//            public void Process(string messageId, DateTime created)
//            {
//                if (IsMessageProcessed(messageId))
//                    return;
//            
//                Action();
//                AddOutboxMessage(messageId, created);
//            }
//
//            public abstract Action()
//            {
//                
//            }
//        }
    }
}