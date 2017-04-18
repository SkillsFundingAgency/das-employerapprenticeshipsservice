using FluentValidation.Attributes;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(ChangeStatusViewModelValidator))]
    public sealed class ChangeStatusViewModel
    {
        public ChangeStatusViewModel()
        {
            DateOfChange = new DateTimeViewModel();
        }

        public ChangeStatusType? ChangeType { get; set; }

        public WhenToMakeChangeOptions WhenToMakeChange { get; set; }

        public DateTimeViewModel DateOfChange { get; set; }

        public bool? ChangeConfirmed { get; set; }
    }
}