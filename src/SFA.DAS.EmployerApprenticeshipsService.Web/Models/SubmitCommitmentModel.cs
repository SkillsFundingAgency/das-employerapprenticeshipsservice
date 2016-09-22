namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class SubmitCommitmentModel
    {
        public long AccountId { get; set; }
        public long CommitmentId { get; set; }
        public string Message { get; set; }
    }
}