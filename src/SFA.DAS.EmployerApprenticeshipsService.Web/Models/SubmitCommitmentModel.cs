namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class SubmitCommitmentModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string Message { get; set; }
    }
}