using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Web.Extensions;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingDeleteApprenticeshipConfirmation : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToGetApprenticeship()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            //Act
            await EmployerCommitmentOrchestrator.GetDeleteApprenticeshipViewModel("ABC123", "EXT789", "ABC321", "ABC456");

            //Assert
            MockMediator.Verify(x=> x.SendAsync(It.Is<GetApprenticeshipQueryRequest>(r=> r.ApprenticeshipId == 456 && r.AccountId == 123)));
        }

        [Test]
        public async Task ShouldReturnApprenticeshipDetails()
        {
            //Arrange
            var expected = new Apprenticeship
            {
                FirstName = "John",
                LastName = "Smith",
                DateOfBirth = new DateTime(1976, 9, 1)
            };

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = expected
                });

            //Act
            var viewModel = await EmployerCommitmentOrchestrator.GetDeleteApprenticeshipViewModel("ABC123", "EXT789", "ABC321", "ABC456");

            //Assert
            Assert.AreEqual(String.Format($"{expected.FirstName} {expected.LastName}"), viewModel.Data.ApprenticeshipName);
            Assert.AreEqual(expected.DateOfBirth.Value.ToGdsFormat(), viewModel.Data.DateOfBirth);
        }
    
    }
}
