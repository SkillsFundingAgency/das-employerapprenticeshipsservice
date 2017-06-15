using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerInformation;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    class WhenSearchingForACompany
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenIShouldGetBackABadRequestIfACompanyCannotBeFound()
        {
            //Assign
            var request = new SelectEmployerViewModel
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

        [Test]
        public async Task ThenTheValuesWillBeCorrectlyMappedInTheResponse()
        {
            //Arrange
            var request = new SelectEmployerViewModel { EmployerRef = "251643" };
            var response = new GetEmployerInformationResponse
            {
                CompanyStatus = "active",
                AddressLine1 = "address1",
                AddressLine2 = "address2",
                AddressPostcode = "ADD123",
                CompanyName = "Company Name",
                CompanyNumber = "ABC12345",
                DateOfIncorporation = DateTime.MaxValue
            };
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerInformationRequest>())).ReturnsAsync(response);

            //Act
            var actual = await _employerAccountOrchestrator.GetCompanyDetails(request);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(response.DateOfIncorporation, actual.Data.DateOfInception);
            Assert.AreEqual(response.CompanyStatus, actual.Data.Status);
            Assert.AreEqual($"{response.AddressLine1}, {response.AddressLine2}, {response.AddressPostcode}", actual.Data.Address);
            Assert.AreEqual(response.CompanyName, actual.Data.Name);
            Assert.AreEqual(response.CompanyNumber, actual.Data.ReferenceNumber);
        }
    }
}
