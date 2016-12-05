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

        public SaveStatus SaveStatus { get; set; }

        public bool ApproveAndSend { get; set; }
    }
}