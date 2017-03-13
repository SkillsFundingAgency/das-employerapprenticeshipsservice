using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingFinishEditing : OrchestratorTestBase
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
            await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            MockMediator.Verify(x => x.SendAsync(It.Is<GetLegalEntityAgreementRequest>(c => c.AccountId == 123L && c.LegalEntityCode=="321" )), Times.Once);
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
            var result = await EmployerCommitmentOrchestrator.GetFinishEditingViewModel("ABC123", "XYZ123", "ABC321");

            //Assert
            Assert.AreEqual(isSigned, result.Data.HasSignedTheAgreement);
        }
    }
}
