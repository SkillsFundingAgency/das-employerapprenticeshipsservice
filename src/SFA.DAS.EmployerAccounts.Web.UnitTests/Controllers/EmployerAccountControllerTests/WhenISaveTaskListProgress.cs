using AutoFixture.NUnit3;
using FluentAssertions;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    class WhenISaveTaskListProgress
    {
        [Test, MoqAutoData]
        public void Then_HashedAccountId_Is_Persisted(string hashedAccountId, [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            controller.CreateAccountProgressSaved(hashedAccountId);

            // Assert
        }

        [Test, MoqAutoData]
        public void WithAccount_ThenReturnTo_ContinueAccountSetup_TaskList(string hashedAccountId, [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = controller.CreateAccountProgressSaved(hashedAccountId) as ViewResult;
            var model = result.Model as CreateAccountProgressSavedViewModel;

            // Assert
            model.ContinueTaskListRouteName.Should().Be(RouteNames.ContinueNewEmployerAccountTaskList);
        }

        [Test, MoqAutoData]
        public void WithoutAccount_ThenReturnTo_NewAccountTaskList([NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = controller.CreateAccountProgressSaved(string.Empty) as ViewResult;
            var model = result.Model as CreateAccountProgressSavedViewModel;

            // Assert
            model.ContinueTaskListRouteName.Should().Be(RouteNames.NewEmpoyerAccountTaskList);
        }
    }
}
