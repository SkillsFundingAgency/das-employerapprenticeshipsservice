using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Types;
using System;

namespace SFA.DAS.EAS.Portal.Client.Database.Models
{
    public class AccountDocument : Document
    {
        [JsonProperty("account")]
        public Account Account { get ; set; }

        [JsonProperty("accountId")]
        public long AccountId => Account.Id;
        private AccountDocument(long accountId) : base(1)
        {
            Id = Guid.NewGuid();
            IsNew = false;            
            Account = new Account() { Id = accountId };
        }

        [JsonConstructor]
        public AccountDocument()
        {         
        }
        public static AccountDocument Create(long accountId)
        {
            return new AccountDocument(accountId) { IsNew = true };
        }
    }
}