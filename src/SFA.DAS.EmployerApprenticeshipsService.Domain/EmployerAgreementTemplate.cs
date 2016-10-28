using System;

namespace SFA.DAS.EAS.Domain
{
    public class EmployerAgreementTemplate
    {
        public int Id { get; set; }
        public string Ref { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReleasedDate { get; set; }
    }
}