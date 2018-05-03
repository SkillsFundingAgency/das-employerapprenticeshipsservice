using System;

namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class AgreementTemplateDto
    {
        public virtual int Id { get; set; }
        public virtual string PartialViewName { get; set; }
        public virtual DateTime? CreatedDate { get; set; }
        public virtual int VersionNumber { get; set; }
    }
}
