using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingtLegalEntitySignedAgreement : OrchestratorTestBase
    {
        [Test]
        public async Task ShouldCallMediatorToGetLegalEntityAgreementRequest()
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetLegalEntityAgreementRequest>()))
                .ReturnsAsync(new GetLegalEntityAgreementResponse
                {
                    EmployerAgreement = new EmployerAgreementView()
                });

            //Act
            await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", "123", "C789");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetLegalEntityAgreementRequest>(c => c.AccountId == 123L && c.LegalEntityCode == "123")), Times.Once);
        }

        [TestCase(true, Description = "The Employer has signed the agreement")]
        [TestCase(false, Description = "The Employer has not signed the agreement")]
        public async Task ThenTheViewModelShouldReflectWhetherTheAgreementHasBeenSigned(bool isSigned)
        {
            //Arrange
            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetLegalEntityAgreementRequest>()))
                .ReturnsAsync(new GetLegalEntityAgreementResponse
                {
                    EmployerAgreement = isSigned ? null : new EmployerAgreementView()
                });

            //Act
            var result = await EmployerCommitmentOrchestrator.GetLegalEntitySignedAgreementViewModel("ABC123", "XYZ123", "C789");

            //Assert
            Assert.AreEqual(isSigned, result.Data.HasSignedAgreement);
        }
    }
}
