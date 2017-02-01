using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Validators
{
    [TestFixture]
    public sealed class WhenValidatingApprenticeshipViewModelForApproval
    {
        private ApprenticeshipViewModelApproveValidator _validator = new ApprenticeshipViewModelApproveValidator();
        private ApprenticeshipViewModel _validModel;

        [SetUp]
        public void Setup()
        {
            _validModel = new ApprenticeshipViewModel
            {
                FirstName = "First Name",
                LastName = "Last Name",
                ULN = "ULN",
                Cost = "COST",
                StartDate = new DateTimeViewModel(1, 5, 2200),
                EndDate = new DateTimeViewModel(1, 5, 2200),
                TrainingId = "5",
                DateOfBirth = new DateTimeViewModel(5, 9, 1882),
                NINumber = "SE000NI00NUKBER"
            };
        }

        [Test]
        public void TestValidationWithEmptyModel()
        {
            var result = _validator.Validate(new ApprenticeshipViewModel());
            result.IsValid.Should().BeFalse();
            result.Errors.Count.Should().Be(8);
        }

        [Test]
        public void TestValidationWithValidModel()
        {
            var result = _validator.Validate(_validModel);
            result.IsValid.Should().BeTrue();
            result.Errors.Count.ShouldBeEquivalentTo(0);
            result.Errors.Count.ShouldBeEquivalentTo(0);
        }
    }
}
