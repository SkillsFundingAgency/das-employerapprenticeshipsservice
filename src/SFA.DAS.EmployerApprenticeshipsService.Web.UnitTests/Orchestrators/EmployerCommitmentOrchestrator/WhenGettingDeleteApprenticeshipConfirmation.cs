using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Application.Queries.GetCommitment;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public class WhenGettingDeleteApprenticeshipConfirmation
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

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new Commitment
                    {
                        EditStatus = EditStatus.EmployerOnly,
                        AgreementStatus = AgreementStatus.NotAgreed
                    }
                });

            _logger = new Mock<ILogger>();
            _calculator = new Mock<ICommitmentStatusCalculator>();

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _hashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _hashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);
            _hashingService.Setup(x => x.DecodeValue("EXT789")).Returns(789L);

            _employerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(_mediator.Object,
                _hashingService.Object, _calculator.Object, _logger.Object);
        }

        [Test]
        public async Task ShouldCallMediatorToGetApprenticeship()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship()
                });

            //Act
            await _employerCommitmentOrchestrator.GetDeleteApprenticeshipViewModel("ABC123", "EXT789", "ABC321", "ABC456");

            //Assert
            _mediator.Verify(x=> x.SendAsync(It.Is<GetApprenticeshipQueryRequest>(r=> r.ApprenticeshipId == 456 && r.AccountId == 123)));
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

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetApprenticeshipQueryRequest>()))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = expected
                });

            //Act
            var viewModel = await _employerCommitmentOrchestrator.GetDeleteApprenticeshipViewModel("ABC123", "EXT789", "ABC321", "ABC456");

            //Assert
            Assert.AreEqual(String.Format($"{expected.FirstName} {expected.LastName}"), viewModel.Data.ApprenticeshipName);
            Assert.AreEqual(expected.DateOfBirth.Value.ToGdsFormat(), viewModel.Data.DateOfBirth);
        }
    
    }
}
