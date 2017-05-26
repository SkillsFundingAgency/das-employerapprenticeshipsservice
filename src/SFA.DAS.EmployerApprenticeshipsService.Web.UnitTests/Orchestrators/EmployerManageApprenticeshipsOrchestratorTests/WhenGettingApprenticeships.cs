using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Queries.ApprenticeshipSearch;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    [TestFixture]
    public class WhenGettingApprenticeships
    {
        private EmployerManageApprenticeshipsOrchestrator _orchestrator;
        private ApprenticeshipMapper _apprenticeshipMapper;
        private Mock<IMediator> _mockMediator;
        private Mock<ICurrentDateTime> _mockDateTime;
        private Mock<ICookieStorageService<UpdateApprenticeshipViewModel>> _cookieStorageService;
        private Mock<IApprenticeshipFiltersMapper> _mockApprenticeshipFiltersMapper;

        [SetUp]
        public void Setup()
        {
            _mockMediator = new Mock<IMediator>();
            _mockDateTime = new Mock<ICurrentDateTime>();

            _cookieStorageService = new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>();

            _apprenticeshipMapper = new ApprenticeshipMapper(Mock.Of<IHashingService>(), _mockDateTime.Object, _mockMediator.Object);

            _mockMediator.Setup(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()))
                .ReturnsAsync(new ApprenticeshipSearchQueryResponse
                {
                    Apprenticeships = new List<Apprenticeship>(),
                    Facets = new Facets()
                });

            _mockApprenticeshipFiltersMapper = new Mock<IApprenticeshipFiltersMapper>();

            _mockApprenticeshipFiltersMapper.Setup(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>()))
                .Returns(new ApprenticeshipSearchQuery());

            _mockApprenticeshipFiltersMapper.Setup(
                x => x.Map(It.IsAny<Facets>()))
                .Returns(new ApprenticeshipFiltersViewModel());

            _orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                _mockMediator.Object,
                Mock.Of<IHashingService>(),
                _apprenticeshipMapper,
                Mock.Of<ApprovedApprenticeshipViewModelValidator>(),
                new CurrentDateTime(),
                Mock.Of<ILogger>(),
                _cookieStorageService.Object,
                _mockApprenticeshipFiltersMapper.Object);
        }

        [Test]
        public async Task ThenShouldMapFiltersToSearchQuery()
        {
            //Act
            await _orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            _mockApprenticeshipFiltersMapper.Verify(
                x => x.MapToApprenticeshipSearchQuery(It.IsAny<ApprenticeshipFiltersViewModel>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldMapSearchResultsToViewModel()
        {
            //Act
            await _orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            _mockApprenticeshipFiltersMapper.Verify(
                x => x.Map(It.IsAny<Facets>())
                , Times.Once());
        }

        [Test]
        public async Task ThenShouldCallMediatorToQueryApprenticeships()
        {
            //Act
            await _orchestrator.GetApprenticeships("hashedAccountId", new ApprenticeshipFiltersViewModel(), "UserId");

            //Assert
            _mockMediator.Verify(x => x.SendAsync(It.IsAny<ApprenticeshipSearchQueryRequest>()), Times.Once);
        }
    }
}
