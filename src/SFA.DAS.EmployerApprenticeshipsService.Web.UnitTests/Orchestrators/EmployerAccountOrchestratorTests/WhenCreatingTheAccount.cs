using System;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateAccount;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    public class WhenCreatingTheAccount
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
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateAccountCommand>()))
                     .ReturnsAsync(new CreateAccountCommandResponse()
                     {
                         HashedAccountId = "ABS10"
                     });
        }

        [Test]
        public async Task ThenTheMediatorCommandIsCalledWithCorrectParameters()
        {
            //Arrange
            var model = ArrangeModel();

            //Act
            await _employerAccountOrchestrator.CreateAccount(model, It.IsAny<HttpContextBase>());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAccountCommand>(
                        c => c.AccessToken.Equals(model.AccessToken)
                        && c.OrganisationDateOfInception.Equals(model.OrganisationDateOfInception)
                        && c.OrganisationName.Equals(model.OrganisationName)
                        && c.OrganisationReferenceNumber.Equals(model.OrganisationReferenceNumber)
                        && c.OrganisationAddress.Equals(model.OrganisationAddress)
                        && c.OrganisationDateOfInception.Equals(model.OrganisationDateOfInception)
                        && c.OrganisationStatus.Equals(model.OrganisationStatus)
                        && c.PayeReference.Equals(model.PayeReference)
                        && c.AccessToken.Equals(model.AccessToken)
                        && c.RefreshToken.Equals(model.RefreshToken)
                        && c.EmployerRefName.Equals(model.EmployerRefName)
                    )));
        }

        [Test]
        public async Task ThenIShouldGetBackTheNewAccountId()
        {
            //Assign
            const string hashedId = "1AFGG0";
            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateAccountCommand>()))
                .ReturnsAsync(new CreateAccountCommandResponse()
                {
                    HashedAccountId = hashedId
                });

            //Act
            var response = await _employerAccountOrchestrator.CreateAccount(new CreateAccountModel(), It.IsAny<HttpContextBase>());

            //Assert
            Assert.AreEqual(hashedId, response.Data?.EmployerAgreement?.HashedAccountId);

        }

        [Test]
        public async Task ThenTheSummaryViewRetrievesCookieData()
        {
            //Arrange
            var employerAccountData = new EmployerAccountData
            {
                OrganisationStatus = "Active",
                OrganisationName = "Test Company",
                OrganisationDateOfInception = DateTime.MaxValue,
                OrganisationReferenceNumber = "ABC12345",
                OrganisationRegisteredAddress = "My Address",
                PayeReference = "123/abc",
                EmpRefNotFound = true
            };

            _cookieService.Setup(x => x.Get(It.IsAny<HttpContextBase>(), It.IsAny<string>()))
                .Returns(JsonConvert.SerializeObject(employerAccountData));

            var context = new Mock<HttpContextBase>();

            //Act
            var model = _employerAccountOrchestrator.GetSummaryViewModel(context.Object);

            //Assert
            Assert.AreEqual(employerAccountData.OrganisationName, model.Data.OrganisationName);
            Assert.AreEqual(employerAccountData.OrganisationStatus, model.Data.OrganisationStatus);
            Assert.AreEqual(employerAccountData.OrganisationReferenceNumber, model.Data.OrganisationReferenceNumber);
            Assert.AreEqual(employerAccountData.PayeReference, model.Data.PayeReference);
            Assert.AreEqual(employerAccountData.EmpRefNotFound, model.Data.EmpRefNotFound);

        }


        private static CreateAccountModel ArrangeModel()
        {
            return new CreateAccountModel
            {
                OrganisationName = "test",
                UserId = Guid.NewGuid().ToString(),
                PayeReference = "123ADFC",
                OrganisationReferenceNumber = "12345",
                OrganisationDateOfInception = new DateTime(2016, 10, 30),
                OrganisationAddress = "My Address",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                OrganisationStatus = "active",
                EmployerRefName = "Scheme 1"
            };
        }
    }
}

