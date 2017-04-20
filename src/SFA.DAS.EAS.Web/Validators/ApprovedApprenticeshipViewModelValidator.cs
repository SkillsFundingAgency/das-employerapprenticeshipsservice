using System.Collections.Generic;
using System.Linq;

using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Validators.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Validators
{
    public class ApprovedApprenticeshipViewModelValidator : ApprenticeshipCoreValidator
    {
        public ApprovedApprenticeshipViewModelValidator()
            : base(new WebApprenticeshipValidationText(), new CurrentDateTime())
        {
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel instance)
        {
            var result = base.Validate(instance);

            return result.Errors.ToDictionary(a => a.PropertyName, b => b.ErrorMessage);
        }
    }
}