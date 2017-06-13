using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Application.Queries.GetApprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.Validators;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public class WhenApprenticeshipsAreStoredInTheCookie
    {
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>  _cookieStorageService;
        private EmployerManageApprenticeshipsOrchestrator _orchestrator;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-apprentices";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _hashingService = new Mock<IHashingService>();
            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _logger = new Mock<ILog>();
            _cookieStorageService = new Mock<ICookieStorageService<UpdateApprenticeshipViewModel>>();

            _orchestrator = new EmployerManageApprenticeshipsOrchestrator(
                _mediator.Object, 
                _hashingService.Object, 
                _apprenticeshipMapper.Object,
                new ApprovedApprenticeshipViewModelValidator(),
                new CurrentDateTime(),
                _logger.Object,
                _cookieStorageService.Object,
                Mock.Of<IApprenticeshipFiltersMapper>());
        }

       
        [Test]
        public void ThenTheCookieIsDeletedBeforeBeingCreated()
        {
            //Arrange
            var model = new UpdateApprenticeshipViewModel();

            //Act
            _orchestrator.CreateApprenticeshipViewModelCookie(model);

            //Assert
            _cookieStorageService.Verify(x=>x.Delete(CookieName));
            _cookieStorageService.Verify(x=>x.Create(model,CookieName,1));

        }

        [Test]
        public async Task ThenTheModelIsPopulatedFromTheCookie()
        {
            //Arrange
            var expectedHashedAccountId = "123456PRDF";
            var expectedHashedApprenticeshipId = "ABCC456";
            var expectedAccountId = 12345;
            var expectedApprenticeshipId = 54321;
            _cookieStorageService.Setup(x => x.Get(CookieName)).Returns(new UpdateApprenticeshipViewModel());
            _hashingService.Setup(x => x.DecodeValue(expectedHashedAccountId)).Returns(expectedAccountId);
            _hashingService.Setup(x => x.DecodeValue(expectedHashedApprenticeshipId)).Returns(expectedApprenticeshipId);
            _mediator.Setup(
                x =>
                    x.SendAsync( It.Is<GetApprenticeshipQueryRequest>(
                                c =>
                                c.AccountId.Equals(expectedAccountId) &&
                                c.ApprenticeshipId.Equals(expectedApprenticeshipId))))
                .ReturnsAsync(new GetApprenticeshipQueryResponse
                {
                    Apprenticeship = new Apprenticeship { }
                });

            //Act
            var actual = await _orchestrator.GetOrchestratorResponseUpdateApprenticeshipViewModelFromCookie(expectedHashedAccountId, expectedHashedApprenticeshipId);

            //Assert
            Assert.IsNotNull(actual);
            _cookieStorageService.Verify(x=>x.Get(CookieName), Times.Once);
            Assert.IsAssignableFrom<OrchestratorResponse<UpdateApprenticeshipViewModel>>(actual);
        }
    }
}
