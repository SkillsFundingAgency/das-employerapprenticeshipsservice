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

        /// <summary>
        ///     The version number of the last agreement template signed by the legal entity for the account.
        /// </summary>
        public virtual int? SignedAgreementVersion { get; set; }

        /// <summary>
        ///     A reference to the most recently signed agreement.
        /// </summary>
        public virtual long? SignedAgreementId { get; set; }

        /// <summary>
        ///     The version number of the latest agreement template that is pending. If this agreement is signed then this 
        ///     property will revert to null.
        /// </summary>
        public virtual int? PendingAgreementVersion { get; set; }

        /// <summary>
        ///     A reference to the pending agreement.
        /// </summary>
        public virtual long? PendingAgreementId { get; set; }

        public virtual long AccountId { get; set; }
        public virtual long LegalEntityId { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }

        public string PublicHashedId { get; set; }

        public virtual Account Account { get; set; }
        public virtual LegalEntity LegalEntity { get; set; }
        public virtual EmployerAgreement SignedAgreement { get; set; }
        public virtual EmployerAgreement PendingAgreement { get; set; }
        public virtual ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();
    }
}
