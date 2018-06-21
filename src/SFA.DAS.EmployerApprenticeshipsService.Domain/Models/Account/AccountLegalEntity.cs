using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    public class AccountLegalEntity
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual long CurrentSignedAgreement { get; set; }
        public virtual long CurrentPendingAgreement { get; set; }
        public virtual long AccountId { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }

        public virtual Account Account { get; set; }
        public virtual LegalEntity LegalEntity { get; set; }
        public virtual ICollection<EmployerAgreement> EmployerAgreements { get; set; } = new List<EmployerAgreement>();
    }
}
