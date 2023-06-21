using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
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
            var model = result.Model as AccountTaskListViewModel;

            // Assert
            Assert.IsNotNull(model);
            model.HasPayeScheme.Should().BeFalse();
            model.CompletedSections.Should().Be(1);
        }

        [Test]
        [MoqAutoData]
        public async Task WhenHashedId_Then_GetCreateAccountTaskList_Decodes_HashedAccountId(
            string hashedAccountId,
            [Frozen] Mock<IEncodingService> encodingService,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange


            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId);

            // Assert
            encodingService.Verify(m => m.Decode(hashedAccountId, EncodingType.AccountId), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task WhenHashedId_Then_GetCreateAccountTaskList_Should_GetAccountPayes(
            string hashedAccountId,
            long accountId,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            encodingService.Setup(m => m.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId);

            // Assert
            mediator.Verify(m => m.Send(It.Is<GetAccountPayeSchemesQuery>(x => x.AccountId == accountId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task Then_GetCreateAccountTaskList_WithoutPaye_Should_Return_HasPaye_False(
            string hashedAccountId,
            long accountId,
            GetAccountPayeSchemesResponse payeSchemeResponse,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            payeSchemeResponse.PayeSchemes.Clear();
            encodingService.Setup(m => m.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
            mediator.Setup(m => m.Send(It.Is<GetAccountPayeSchemesQuery>(x => x.AccountId == accountId), It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as AccountTaskListViewModel;

            // Assert
            Assert.IsNotNull(model);
            model.HasPayeScheme.Should().BeFalse();
            model.CompletedSections.Should().Be(1);
        }

        [Test]
        [InlineAutoData(1)]
        [InlineAutoData(3)]
        public async Task Then_GetCreateAccountTaskList_AccountHasPayeSchemes_Should_Return_HasPaye_True(
            int payeCount,
            string hashedAccountId,
            long accountId,
            GetAccountPayeSchemesResponse payeSchemeResponse,
            [Frozen] Mock<IMediator> mediator,
            [Frozen] Mock<IEncodingService> encodingService,
            [NoAutoProperties] EmployerAccountController controller)
        {
            // Arrange
            payeSchemeResponse.PayeSchemes = payeSchemeResponse.PayeSchemes.Take(payeCount).ToList();
            encodingService.Setup(m => m.Decode(hashedAccountId, EncodingType.AccountId)).Returns(accountId);
            mediator.Setup(m => m.Send(It.Is<GetAccountPayeSchemesQuery>(x => x.AccountId == accountId), It.IsAny<CancellationToken>())).ReturnsAsync(payeSchemeResponse);

            // Act
            var result = await controller.CreateAccountTaskList(hashedAccountId) as ViewResult;
            var model = result.Model as AccountTaskListViewModel;

            // Assert
            Assert.IsNotNull(model);
            model.HasPayeScheme.Should().BeTrue();
            model.CompletedSections.Should().Be(2);
        }
    }
}
