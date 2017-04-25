using MediatR;
using Moq;
using NLog;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.EAS.Application.Queries.GetCommitment;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    public abstract class OrchestratorTestBase
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IHashingService> _mockHashingService;
        private Mock<ICommitmentStatusCalculator> _mockCalculator;
        private Mock<IApprenticeshipMapper> _mockApprenticeshipMapper;
        private Mock<ICommitmentMapper> _mockCommitmentMapper;

        protected Mock<IMediator> MockMediator;
        protected EmployerCommitmentsOrchestrator EmployerCommitmentOrchestrator;

        [SetUp]
        public void Setup()
        {
            MockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger>();
            _mockCalculator = new Mock<ICommitmentStatusCalculator>();
            _mockApprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _mockCommitmentMapper = new Mock<ICommitmentMapper>();

            _mockHashingService = new Mock<IHashingService>();
            _mockHashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC321")).Returns(321L);
            _mockHashingService.Setup(x => x.DecodeValue("ABC456")).Returns(456L);

            MockMediator.Setup(x => x.SendAsync(It.IsAny<GetCommitmentQueryRequest>()))
                .ReturnsAsync(new GetCommitmentQueryResponse
                {
                    Commitment = new CommitmentView
                    {
                        Id = 123,
                        LegalEntityId = "321",
                        EditStatus = EditStatus.EmployerOnly
                    }
                });

            EmployerCommitmentOrchestrator = new EmployerCommitmentsOrchestrator(
                MockMediator.Object,
                _mockHashingService.Object, 
                _mockCalculator.Object, 
                _mockApprenticeshipMapper.Object, 
                _mockCommitmentMapper.Object, 
                _mockLogger.Object);
        }
    }
}
