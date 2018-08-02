using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models
{
    public class AgreementTemplate
    {
        public virtual int Id { get; set; }
        public virtual ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();
        public virtual DateTime? CreatedDate { get; set; }
        public virtual string PartialViewName { get; set; }
        public virtual int VersionNumber { get; set; }
    }
}