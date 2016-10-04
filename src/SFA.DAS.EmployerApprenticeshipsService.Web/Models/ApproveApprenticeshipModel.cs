namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class ApproveApprenticeshipModel
    {
        public string HashedAccountId { get; set; }
        public long CommitmentId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}