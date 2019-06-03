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
        private AccountDocument(long accountId) : base(1)
        {
            Id = Guid.NewGuid();
            Account = new Account { Id = accountId };
        }

        [JsonConstructor]
        public AccountDocument()
        {         
        }
        
        //todo: don't hink this gives us anything, just make private ctor public
        public static AccountDocument Create(long accountId)
        {
            return new AccountDocument(accountId) { IsNew = true };
        }
    }
}