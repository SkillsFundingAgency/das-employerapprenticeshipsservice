using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.AccountName
{
    [TestFixture]
    class WhenIDoNotSelectAChoice
    {
        [Test, MoqAutoData]
        public async Task Then_Return_Error(
            string hashedAccountId,
            RenameEmployerAccountViewModel viewModel,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            viewModel.ChangeAccountName = null;

            // Act
            var result = await controller.AccountName(hashedAccountId, viewModel) as ViewResult;
            var model = result.Model.As<OrchestratorResponse<RenameEmployerAccountViewModel>>();

            // Assert
            model.Status.Should().Be(HttpStatusCode.BadRequest);
            model.Data.ErrorDictionary.Should().NotBeEmpty();   
        }

        [Test, MoqAutoData]
        public async Task Then_Do_Not_Name_Account(
            string hashedAccountId,
            RenameEmployerAccountViewModel viewModel,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            viewModel.ChangeAccountName = null;
            
            // Act
            var result = await controller.AccountName(hashedAccountId, viewModel) as ViewResult;
            var model = result.Model.As<OrchestratorResponse<RenameEmployerAccountViewModel>>();

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<RenameEmployerAccountCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
