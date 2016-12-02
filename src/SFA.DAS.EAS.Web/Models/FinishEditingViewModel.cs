using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Web.Models
{
    using SFA.DAS.EAS.Web.Models.Types;

    public sealed class FinishEditingViewModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option")]
        public SaveStatus SaveStatus { get; set; }

        public bool ApproveAndSend { get; set; }
    }
}