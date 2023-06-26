using AutoFixture.NUnit3;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.RenameAccount
{
    [TestFixture]
    class WhenISelectToNotChangeAccountName
    {
        [Test, MoqAutoData]
        public async Task Then_Name_Confirmed_Is_Set(
            string hashedAccountId,
            RenameEmployerAccountViewModel viewModel,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = await controller.RenameAccount(hashedAccountId, viewModel) as ViewResult;

            // Assert
        }
    }
}
