namespace SFA.DAS.EAS.Web.Models
{
    public sealed class SubmitCommitmentViewModel
    {
        public CommitmentViewModel Commitment { get; set; }
        public SubmitCommitmentModel SubmitCommitmentModel { get; set; }
        public string SaveOrSend { get; set; }
    }
}