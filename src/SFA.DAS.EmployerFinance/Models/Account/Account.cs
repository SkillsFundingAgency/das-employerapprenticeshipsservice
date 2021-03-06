using System.Collections.Generic;

namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class Account
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();
    }
}