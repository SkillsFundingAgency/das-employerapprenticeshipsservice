using System;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
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
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.CompanyName.Equals(model.CompanyName)
                        && c.CompanyNumber.Equals(model.CompanyNumber)
                        && c.CompanyRegisteredAddress.Equals(model.CompanyRegisteredAddress)
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.CompanyStatus.Equals(model.CompanyStatus)
                        && c.EmployerRef.Equals(model.EmployerRef)
                        && c.AccessToken.Equals(model.AccessToken)
                        && c.RefreshToken.Equals(model.RefreshToken)
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
        
        private static CreateAccountModel ArrangeModel()
        {
            return new CreateAccountModel
            {
                CompanyName = "test",
                UserId = Guid.NewGuid().ToString(),
                EmployerRef = "123ADFC",
                CompanyNumber = "12345",
                CompanyDateOfIncorporation = new DateTime(2016, 10, 30),
                CompanyRegisteredAddress = "My Address",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                CompanyStatus = "active"
            };
        }
    }
}

