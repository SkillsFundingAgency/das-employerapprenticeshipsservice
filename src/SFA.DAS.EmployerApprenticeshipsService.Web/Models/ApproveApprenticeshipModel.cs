namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class ApproveApprenticeshipModel
    {
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string Message { get; set; }
    }
}