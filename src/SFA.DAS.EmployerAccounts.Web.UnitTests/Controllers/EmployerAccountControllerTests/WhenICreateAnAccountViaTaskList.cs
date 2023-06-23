using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenICreateAnAccountViaTaskList
    {
        [Test]
        [MoqAutoData]
        public async Task When_No_HashedId_Then_Do_Not_Fetch_Account_Status(
            [Frozen] Mock<IEncodingService> encodingService,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = await controller.CreateAccountTaskList(null) as ActionResult;

            // Assert
            encodingService.Verify(m => m.Decode(It.IsAny<string>(), EncodingType.AccountId), Times.Never);
        }

        [Test]
        [MoqAutoData]
        public async Task When_No_HashedId_Then_HasPaye_False(
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = await controller.CreateAccountTaskList(null) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            Assert.IsNotNull(model);
            model.Data.HasPayeScheme.Should().BeFalse();
            model.Data.CompletedSections.Should().Be(1);
        }

        [Test]
        [MoqAutoData]
        public async Task WhenHashedId_Then_GetCreateAccountTaskList_Decodes_HashedAccountId(
            string hashedAccountId,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId);

            // Assert
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Test]
        [MoqAutoData]
        public async Task WhenHashedId_And_Account_NotFound_Then_Return_NotFound(
            string hashedAccountId,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;

            // Assert
            result.ViewName.Should().Be(ControllerConstants.NotFoundViewName);
        }

        [Test]
        [MoqAutoData]
        public async Task WhenHashedId_Then_GetCreateAccountTaskList_Sets_ViewModel_HashedAccountId(
            string hashedAccountId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDetailResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            model.Data.HashedAccountId.Should().Be(hashedAccountId);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_GetCreateAccountTaskList_WithoutPaye_Should_Return_HasPaye_False(
            string hashedAccountId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            accountDetailResponse.Account.PayeSchemes.Clear();
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDetailResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            Assert.IsNotNull(model);
            model.Data.HasPayeScheme.Should().BeFalse();
            model.Data.CompletedSections.Should().Be(1);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_GetCreateAccountTaskList_AccountHasPayeSchemes_Should_Return_HasPaye_True(
            string hashedAccountId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            accountDetailResponse.Account.PayeSchemes = accountDetailResponse.Account.PayeSchemes.Take(1).ToList();
            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDetailResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            Assert.IsNotNull(model);
            model.Data.HasPayeScheme.Should().BeTrue();
            model.Data.CompletedSections.Should().Be(2);
        }
    }
}
