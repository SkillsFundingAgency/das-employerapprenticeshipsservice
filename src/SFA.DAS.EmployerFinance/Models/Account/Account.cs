using System.Collections.Generic;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class Account
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();
    }
}