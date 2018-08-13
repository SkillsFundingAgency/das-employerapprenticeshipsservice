using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Models.Account
{
    public class LegalEntity
    {
        public virtual long Id { get; set; }
        public virtual ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();
        public virtual string Code { get; set; }
        public virtual DateTime? DateOfIncorporation { get; set; }
        public virtual string Name { get; set; }
        public virtual byte? PublicSectorDataSource { get; set; }
        public virtual string RegisteredAddress { get; set; }
        public virtual string Sector { get; set; }
        public virtual byte Source { get; set; }
        public virtual string Status { get; set; }
    }
}