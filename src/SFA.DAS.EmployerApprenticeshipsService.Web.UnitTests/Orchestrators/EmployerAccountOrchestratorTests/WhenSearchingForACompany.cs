using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetEmployerInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    class WhenSearchingForACompany
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieService> _cookieService;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfACompanyCannotBeFound()
        {
            //Assign
            var request = new SelectEmployerModel
            {
                EmployerRef = "251643"
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerInformationRequest>())).ReturnsAsync(null);

            //Act
            var response = await _employerAccountOrchestrator.GetCompanyDetails(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
            _mediator.Verify(x => x.SendAsync(It.Is<GetEmployerInformationRequest>( info => info.Id.Equals(request.EmployerRef))));
        }
    }
}
