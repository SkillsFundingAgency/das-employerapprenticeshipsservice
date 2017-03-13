using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenDeletingApprenticeship : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToDelete()
        {
            //Arrange
            var model = new DeleteApprenticeshipConfirmationViewModel
            {
                HashedAccountId = "ABC123",
                HashedCommitmentId = "ABC321",
                HashedApprenticeshipId = "ABC456"
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            MockMediator.Setup(x => x.SendAsync(It.IsAny<DeleteApprenticeshipCommand>())).ReturnsAsync(new Unit());

            //Act
            await EmployerCommitmentOrchestrator.DeleteApprenticeship(model, "externalUserId");

            //Assert
            MockMediator.Verify(x=> x.SendAsync(It.Is<DeleteApprenticeshipCommand>(c=> c.AccountId == 123L && c.ApprenticeshipId == 456L)), Times.Once);
        }
    }
}
