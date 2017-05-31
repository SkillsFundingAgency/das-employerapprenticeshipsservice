using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmentOrchestrator
{
    [TestFixture]
    public sealed class WhenGettingIndexView
    {
        private Mock<IMediator> _mediator;
        private EmployerCommitmentsOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger>();
            var calculator = new Mock<ICommitmentStatusCalculator>();
            var hashingService = new Mock<IHashingService>();

            hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
    
            _orchestrator = new EmployerCommitmentsOrchestrator(
                _mediator.Object,
                hashingService.Object, 
                Mock.Of<ICommitmentStatusCalculator>(), 
                Mock.Of<IApprenticeshipMapper>(), 
                Mock.Of<ICommitmentMapper>(),
                logger.Object);
        }

        [Test]
        public async Task ShouldIndicateToHidePaymentPriorityLinkWhenLessThan2ValidProviders()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" }
                    }
                });

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowSetPaymentPriorityLink.Should().BeFalse();
        }

        [Test]
        public async Task ShouldIndicateToShowPaymentPriorityLinkWhen2OrMoreValidProviders()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 2, ProviderId = 22, ProviderName = "CCC" },
                    }
                });

            var response = await _orchestrator.GetIndexViewModel("123", "user123");

            response.Data.ShowSetPaymentPriorityLink.Should().BeTrue();
        }
    }
}
