using System.Collections.Generic;
using System.Linq;

using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;

using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

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
            first.InitialOrder.Should().Be(1);
            first.NewOrder.Should().Be(1);
        }

        [Test]
        public void ShouldMapFromViewModel()
        {
            var inputData = new List<PaymentOrderItem>
                {
                    new PaymentOrderItem
                        { ProviderId = 111L, InitialOrder = 1, NewOrder = 3, ProviderName = "Provider 1"},
                    new PaymentOrderItem
                        { ProviderId = 222L, InitialOrder = 2, NewOrder = 2, ProviderName = "Provider 2"},
                    new PaymentOrderItem
                        { ProviderId = 333L, InitialOrder = 3, NewOrder = 1, ProviderName = "Provider 3"}
                };

            var mapped = _sut.MapPayment(inputData);

            mapped.Count.Should().Be(3);
            var p1 = mapped.Single(m => m.ProviderId == 111);
            var p2 = mapped.Single(m => m.ProviderId == 222);
            var p3 = mapped.Single(m => m.ProviderId == 333);

            p1.ProviderName.Should().Be("Provider 1");
            p1.PriorityOrder.Should().Be(3);

            p2.ProviderName.Should().Be("Provider 2");
            p2.PriorityOrder.Should().Be(2);

            p3.ProviderName.Should().Be("Provider 3");
            p3.PriorityOrder.Should().Be(1);
        }
    }
}
