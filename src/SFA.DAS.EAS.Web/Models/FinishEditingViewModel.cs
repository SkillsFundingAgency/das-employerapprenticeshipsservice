using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.EAS.Web.Models.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{

    [Validator(typeof(FinishEditingViewModelValidator))]
    public sealed class FinishEditingViewModel
    {
        public string HashedAccountId { get; set; }

        public string HashedCommitmentId { get; set; }

        public bool HasApprenticeships { get; set; }

        public int InvalidApprenticeshipCount { get; set; }

        public SaveStatus SaveStatus { get; set; }

        public ApprovalState ApprovalState { get; set; }

        public bool NotReadyForApproval { get;  set; }

        public bool IsApproveAndSend => ApprovalState == ApprovalState.ApproveAndSend;
    }

    public enum ApprovalState
    {
        ApproveAndSend = 0,
        ApproveOnly = 1
    }
}