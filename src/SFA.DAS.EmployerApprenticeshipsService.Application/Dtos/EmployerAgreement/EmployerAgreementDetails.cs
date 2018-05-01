namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class EmployerAgreementDetails
    {
        public long Id { get; set; }
        public int TemplateId { get; set; }
        public string PartialViewName { get; set; }
        public string HashedAgreementId { get; set; }
        public int VersionNumber { get; set; }
    }
}