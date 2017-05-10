using System.ComponentModel.DataAnnotations;

using FluentValidation.Attributes;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(UpdateApprenticeshipViewModelValidator))]
    public class UpdateApprenticeshipViewModel : ViewModelBase
    {
        public string HashedApprenticeshipId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; } = new DateTimeViewModel(0);

        public TrainingType? TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }

        public DateTimeViewModel EndDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmployerRef { get; set; }

        public ApprenticeshipDetailsViewModel OriginalApprenticeship { get; set; }

        public bool? ChangesConfirmed { get; set; }

        public string HashedAccountId { get; set; }

        public string CurrentTableHeadingText { get; set; }

        public string ProviderName { get; set; }

        public string ChangesConfirmedError => GetErrorMessage(nameof(ChangesConfirmed));

        public bool IsDataLockOrigin { get; set; }
    }
}