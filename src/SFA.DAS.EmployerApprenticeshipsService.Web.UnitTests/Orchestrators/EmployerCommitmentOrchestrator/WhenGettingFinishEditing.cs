using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityAgreement;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using NLog;

using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.Validation;
using SFA.DAS.EAS.Application.Queries.GetCommitment;
using SFA.DAS.EAS.Application.Queries.GetOverlappingApprenticeships;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
﻿using SFA.DAS.EAS.Web.Orchestrators.Mappers;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingFinishEditing : OrchestratorTestBase
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IHashingService> _hashingService;
        private Mock<ICommitmentStatusCalculator> _calculator;

        private EmployerCommitmentsOrchestrator _employerCommitmentOrchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _calculator = new Mock<ICommitmentStatusCalculator>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _hashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _hashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Id = 123,
                        LegalEntityId = "321",
                        EditStatus = EditStatus.EmployerOnly
                    }
                });
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetOverlappingApprenticeshipsQueryRequest>()))
                .ReturnsAsync(
                    new GetOverlappingApprenticeshipsQueryResponse { Overlaps = Enumerable.Empty<ApprenticeshipOverlapValidationResult>() });

            _employerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(
                _mediator.Object,
                _hashingService.Object, 
                _calculator.Object, 
                Mock.Of<IApprenticeshipMapper>(), 
                Mock.Of<ICommitmentMapper>(), 
                _logger.Object);
        }

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
