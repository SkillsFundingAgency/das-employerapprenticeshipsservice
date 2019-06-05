using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Types;
using System;

namespace SFA.DAS.EAS.Portal.Client.Database.Models
{
    public class AccountDocument : Document
    {
        [JsonProperty("account")]
        public Account Account { get ; set; }

        [JsonProperty("accountId")]
        public long AccountId => Account.Id;
        public AccountDocument(long accountId) : base(1)
        {
            Id = Guid.NewGuid();
            IsNew = true;
            Account = new Account { Id = accountId };
        }

        [JsonConstructor]
        private AccountDocument()
        {         
        }
    }
}