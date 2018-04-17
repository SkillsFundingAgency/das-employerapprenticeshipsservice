using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Domain.Data.Entities.Account
{
    public class AgreementTemplate
    {
        public virtual int Id { get; set; }
        public virtual string PartialViewName { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual int VersionNumber { get; set; }

        public virtual ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();
    }
}