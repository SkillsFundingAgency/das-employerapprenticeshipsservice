using FluentValidation.Attributes;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    [Validator(typeof(UpdateApprenticeshipViewModelValidator))]
    public class UpdateApprenticeshipViewModel : ApprenticeshipViewModel
    {
        
        public Apprenticeship OriginalApprenticeship { get; set; }

        public bool? ChangesConfirmed { get; set; }
        
    }
}