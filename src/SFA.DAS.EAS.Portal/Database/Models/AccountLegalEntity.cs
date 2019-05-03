using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public class AccountLegalEntity
    {
        //todo: rename to id & name?
        [JsonProperty("accountLegalEntityId")]
        public long AccountLegalEntityId { get; private set; }

        [JsonProperty("legalEntityName")]
        public string LegalEntityName { get; private set; }
        
        [JsonProperty("reservedFundings")]
        public IEnumerable<ReservedFunding> ReservedFundings => _reservedFundings;

        [JsonIgnore]
        private readonly List<ReservedFunding> _reservedFundings = new List<ReservedFunding>();

        private AccountLegalEntity(long accountLegalEntityId, string legalEntityName)
        {
            AccountLegalEntityId = accountLegalEntityId;
            LegalEntityName = legalEntityName;
        }

        public AccountLegalEntity(long accountLegalEntityId, string legalEntityName,
            long reservationId, long courseId, string courseName, DateTime startDate, DateTime endDate)
            : this(accountLegalEntityId, legalEntityName)
        {
            AddReserveFunding(reservationId, courseId, courseName, startDate, endDate);
        }

        public void AddReserveFunding(long reservationId, long courseId, string courseName, DateTime startDate, DateTime endDate)
        {
            _reservedFundings.Add(new ReservedFunding(reservationId, courseId, courseName, startDate, endDate));
        }
    }
}