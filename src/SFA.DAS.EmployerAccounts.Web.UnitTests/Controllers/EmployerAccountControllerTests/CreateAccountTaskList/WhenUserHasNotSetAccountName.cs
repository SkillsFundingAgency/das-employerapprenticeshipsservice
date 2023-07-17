using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.TestCommon.AutoFixture;
using SFA.DAS.EmployerAccounts.Web.RouteValues;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenUserHasNotSetAccountName
    {
        [Test]
        [MoqAutoData]
        public async Task Then_GetCreateAccountTaskList_GetAccountWithId(
            string hashedAccountId,
            string userId,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId);

            // Assert
            mediatorMock.Verify(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [DomainAutoData]
        public async Task And_AccountId_Not_Supplied_Then_Should_Get_PAYE(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(null) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAccountPayeSchemesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_CannotAddAnotherPaye(
            string hashedAccountId,
            string userId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDetailResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            model.Data.AddPayeRouteName.Should().Be(RouteNames.AddPayeShutter);
        }

        [Test]
        [MoqAutoData]
        public async Task And_No_Account_Then_Return_NotFound(
            string hashedAccountId,
            string userId,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;

            // Assert
            result.ViewName.Should().Be(ControllerConstants.NotFoundViewName);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_GetCreateAccountTaskList_Sets_ViewModel_HashedAccountId(
            string hashedAccountId,
            string userId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

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
        public async Task Then_SaveProgressRoute_Maintains_AccountContext(
            string hashedAccountId,
            string userId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

            mediatorMock
                .Setup(m => m.Send(It.Is<GetEmployerAccountDetailByHashedIdQuery>(x => x.HashedAccountId == hashedAccountId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountDetailResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            model.Data.SaveProgressRouteName.Should().Be(RouteNames.PartialAccountSaveProgress);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_AccountHasPayeSchemes_Should_Return_HasPaye_True(
            string hashedAccountId,
            string userId,
            GetEmployerAccountDetailByHashedIdResponse accountDetailResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            SetControllerContextUserIdClaim(userId, controller);

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

        private static void SetControllerContextUserIdClaim(string userId, EmployerAccountController controller)
        {
            var claims = new List<Claim> { new Claim(ControllerConstants.UserRefClaimKeyName, userId) };
            var claimsIdentity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(claimsIdentity);
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }
    }
}
