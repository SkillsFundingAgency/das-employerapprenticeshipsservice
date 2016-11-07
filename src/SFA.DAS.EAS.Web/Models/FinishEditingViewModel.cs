using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class FinishEditingViewModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }

        [Required(ErrorMessage = "Select an option.")]
        public string SaveOrSend { get; set; }
    }
}