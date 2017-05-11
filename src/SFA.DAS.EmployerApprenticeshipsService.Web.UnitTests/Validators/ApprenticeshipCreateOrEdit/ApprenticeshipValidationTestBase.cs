using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.Validators.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Validators.ApprenticeshipCreateOrEdit
{
    public abstract class ApprenticeshipValidationTestBase
    {
        protected readonly Mock<ICurrentDateTime> CurrentDateTime = new Mock<ICurrentDateTime>();

        protected ApprenticeshipViewModelValidator Validator;
        protected ApprenticeshipViewModel ValidModel;

        [SetUp]
        public void BaseSetup()
        {
            CurrentDateTime.Setup(x => x.Now).Returns(new DateTime(2018, 5, 1));

            Validator = new ApprenticeshipViewModelValidator(new WebApprenticeshipValidationText(), CurrentDateTime.Object, new AcademicYear(CurrentDateTime.Object));

            ValidModel = new ApprenticeshipViewModel { ULN = "1001234567", FirstName = "TestFirstName", LastName = "TestLastName" };
        }
    }
}
