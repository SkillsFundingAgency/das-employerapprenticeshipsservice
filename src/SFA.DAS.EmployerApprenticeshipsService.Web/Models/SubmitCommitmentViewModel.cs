using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class SubmitCommitmentViewModel
    {
        public CommitmentViewModel Commitment { get; set; }
        public SubmitCommitmentModel SubmitCommitmentModel { get; set; }
        public string SaveOrSend { get; set; }
    }
}