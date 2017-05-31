using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenMappingProviderPaymentPriority
    {
        private ApprenticeshipMapper _sut;

        [SetUp]
        public void SetUp()
        {
            var hashingService = new Mock<IHashingService>();
            var currentDateTime = new Mock<ICurrentDateTime>();
            var mediator = new Mock<IMediator>();
            _sut = new ApprenticeshipMapper(hashingService.Object, currentDateTime.Object, mediator.Object);
        }

        [Test]
        public void ShouldMapToViewModel()
        {
            var inputData = new List<ProviderPaymentPriorityItem>
                                {
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 111L, PriorityOrder = 1, ProviderName = "Provider 1"},
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 222L, PriorityOrder = 2, ProviderName = "Provider 2"},
                                    new ProviderPaymentPriorityItem
                                        { ProviderId = 333L, PriorityOrder = 3, ProviderName = "Provider 3"}
                                };
            var mapped = _sut.MapPayment(inputData);

            mapped.Items.Count().Should().Be(3);
            var first = mapped.Items.Single(m => m.ProviderId == 111);
            first.ProviderName.Should().Be("Provider 1");
            first.Priority.Should().Be(1);
        }
    }
}
