using FluentAssertions;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;
using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerCommitmEmployerManageApprenticeshipsOrchestratorTestsEmployerManageApprenticeshipsOrchestratorTestsentOrchestrator
{
    [TestFixture]
    public sealed class WhenGettingProviderPriorityList
    {
        private Mock<IMediator> _mediator;
        private EmployerManageApprenticeshipsOrchestrator _orchestrator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger>();
            var hashingService = new Mock<IHashingService>();
            hashingService.Setup(x => x.DecodeValue("ABC123")).Returns(123L);
            var apprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), new CurrentDateTime(), _mediator.Object);

            _orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                _mediator.Object,
                Mock.Of<IHashingService>(),
                apprenticeshipMapper,
                Mock.Of<ApprovedApprenticeshipViewModelValidator>(),
                new CurrentDateTime(),
                Mock.Of<ILogger>(),
                new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>().Object,
                Mock.Of<IApprenticeshipFiltersMapper>());
        }

        [Test]
        public async Task ReturnListOfProvidersInAlphabeticalOrder()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem>
                    {
                        new ProviderPaymentPriorityItem { PriorityOrder = 1, ProviderId = 11, ProviderName = "BBB" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 2, ProviderId = 22, ProviderName = "CCC" },
                        new ProviderPaymentPriorityItem { PriorityOrder = 3, ProviderId = 33, ProviderName = "AAA" },
                    }
                });

            var response = await _orchestrator.GetPaymentOrder("123", "user123");

            var list = response.Data.Items.ToList();
            list[0].ProviderName.Should().Be("AAA");
            list[1].ProviderName.Should().Be("BBB");
            list[2].ProviderName.Should().Be("CCC");
        }

        [Test]
        public async Task ReturnNotFoundIfLessThan2ProvidersInTheList()
        {
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetProviderPaymentPriorityRequest>()))
                .ReturnsAsync(new GetProviderPaymentPriorityResponse
                {
                    Data = new List<ProviderPaymentPriorityItem> { new ProviderPaymentPriorityItem() }
                });

            var response = await _orchestrator.GetPaymentOrder("123", "user123");

            response.Status.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
