using System.Security.Claims;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.TestCommon.AutoFixture;
using SFA.DAS.EmployerAccounts.Web.RouteValues;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.EmployerAccountControllerTests.CreateAccountTaskList
{
    [TestFixture]
    public class WhenUserHasNotAddedPayeAndOrganisation
    {
        [Test]
        [DomainAutoData]
        public async Task Then_GetUserAccounts(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            queryResponse.Accounts.AccountList.Clear();
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(null) as ActionResult;

            // Assert
            mediatorMock.Verify(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [DomainAutoData]
        public async Task Then_Should_Not_Get_PAYE(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            queryResponse.Accounts.AccountList.Clear();
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(null) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAccountPayeSchemesQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        [DomainAutoData]
        public async Task Then_HasPaye_False(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            queryResponse.Accounts.AccountList.Clear();
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(null) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            Assert.IsNotNull(model);
            model.Data.HasPayeScheme.Should().BeFalse();
            model.Data.CompletedSections.Should().Be(1);
        }

        [Test]
        [DomainAutoData]
        public async Task Then_CannotAddAnotherPaye(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            queryResponse.Accounts.AccountList.Clear();
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(string.Empty) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            model.Data.AddPayeRouteName.Should().Be(RouteNames.EmployerAccountPayBillTriage);
        }

        [Test]
        [DomainAutoData]
        public async Task Then_SaveProgressRoute_Without_AccountContext(
            string userId,
            GetUserAccountsQueryResponse queryResponse,
            [Frozen] Mock<IMediator> mediatorMock,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            queryResponse.Accounts.AccountList.Clear();
            SetControllerContextUserIdClaim(userId, controller);
            mediatorMock.Setup(m => m.Send(It.Is<GetUserAccountsQuery>(q => q.UserRef == userId), It.IsAny<CancellationToken>())).ReturnsAsync(queryResponse);

            // Act
            var result = await controller.CreateAccountTaskList(string.Empty) as ViewResult;
            var model = result.Model as OrchestratorResponse<AccountTaskListViewModel>;

            // Assert
            model.Data.SaveProgressRouteName.Should().Be(RouteNames.NewAccountSaveProgress);
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
