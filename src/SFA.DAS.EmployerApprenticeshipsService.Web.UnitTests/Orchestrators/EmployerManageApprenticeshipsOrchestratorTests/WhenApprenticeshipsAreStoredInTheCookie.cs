using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Orchestrators.Mappers;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerManageApprenticeshipsOrchestratorTests
{
    public class WhenApprenticeshipsAreStoredInTheCookie
    {
        private Mock<IMediator> _mediator;
        private Mock<IHashingService> _hashingService;
        private Mock<IApprenticeshipMapper> _apprenticeshipMapper;
        private Mock<ILogger> _logger;
        private Mock<ICookieStorageService<ApprenticeshipViewModel>>  _cookieStorageService;
        private EmployerManageApprenticeshipsOrchestrator _orchestrator;

        private const string CookieName = "sfa-das-employerapprenticeshipsservice-apprentices";

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _hashingService = new Mock<IHashingService>();
            _apprenticeshipMapper = new Mock<IApprenticeshipMapper>();
            _logger = new Mock<ILogger>();
            _cookieStorageService = new Mock<ICookieStorageService<ApprenticeshipViewModel>>();

            _orchestrator = new EmployerManageApprenticeshipsOrchestrator(_mediator.Object, _hashingService.Object, _apprenticeshipMapper.Object, _logger.Object, _cookieStorageService.Object);
        }

        [Test]
        public void ThenTheModelIsReadFromTheCookie()
        {
            //Act
            var actual = _orchestrator.GetUpdateApprenticeshipViewModelFromCookie();

            //Assert
            _cookieStorageService.Verify(x=>x.Get(CookieName));
            Assert.IsAssignableFrom<OrchestratorResponse<UpdateApprenticeshipViewModel>>(actual);
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
    }
}
