using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Models;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.Encoding;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests.Given_No_User_Account_Created
{
    public class WhenCreatingTheAccount
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<EmployerAccountOrchestrator>> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

        private EmployerAccountsConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<EmployerAccountOrchestrator>>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _configuration = new EmployerAccountsConfiguration();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, Mock.Of<IEncodingService>());
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>()))
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
            await _employerAccountOrchestrator.CreateOrUpdateAccount(model, It.IsAny<HttpContext>());

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAccountCommand>(
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
                    ), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenIShouldGetBackTheNewAccountId()
        {
            //Assign
            const string hashedId = "1AFGG0";
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateAccountCommandResponse()
                {
                    HashedAccountId = hashedId
                });

            //Act
            var response = await _employerAccountOrchestrator.CreateOrUpdateAccount(new CreateAccountModel(), It.IsAny<HttpContext>());

            //Assert
            Assert.AreEqual(hashedId, response.Data?.EmployerAgreement?.HashedAccountId);

        }

        [Test]
        public void ThenTheSummaryViewRetrievesCookieData()
        {
            //Arrange
            var employerAccountData = new EmployerAccountData
            {
                EmployerAccountOrganisationData = new EmployerAccountOrganisationData
                { 
                OrganisationStatus = "Active",
                OrganisationName = "Test Company",
                OrganisationDateOfInception = DateTime.MaxValue,
                OrganisationReferenceNumber = "ABC12345",
                OrganisationRegisteredAddress = "My Address",
                },
                EmployerAccountPayeRefData = new EmployerAccountPayeRefData
                { 
                PayeReference = "123/abc",
                EmployerRefName = "Test Scheme 1",
                EmpRefNotFound = true
                }
            };

            _cookieService.Setup(x => x.Get( It.IsAny<string>()))
                .Returns(employerAccountData);

            var context = new Mock<HttpContext>();

            //Act
            var model = _employerAccountOrchestrator.GetSummaryViewModel(context.Object);

            //Assert
            Assert.AreEqual(employerAccountData.EmployerAccountOrganisationData.OrganisationName, model.Data.OrganisationName);
            Assert.AreEqual(employerAccountData.EmployerAccountOrganisationData.OrganisationStatus, model.Data.OrganisationStatus);
            Assert.AreEqual(employerAccountData.EmployerAccountOrganisationData.OrganisationReferenceNumber, model.Data.OrganisationReferenceNumber);
            Assert.AreEqual(employerAccountData.EmployerAccountPayeRefData.PayeReference, model.Data.PayeReference);
            Assert.AreEqual(employerAccountData.EmployerAccountPayeRefData.EmployerRefName, model.Data.EmployerRefName);
            Assert.AreEqual(employerAccountData.EmployerAccountPayeRefData.EmpRefNotFound, model.Data.EmpRefNotFound);

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

