using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.Validators.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly ApprenticeshipViewModelValidator Validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText(), new CurrentDateTime());
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
