﻿using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetLatestAccountAgreementTemplate;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
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
                         HashedId = "ABS10"
                     });
        }

        [Test]
        public async Task ThenTheMediatorCommandIsCalledWithCorrectParameters()
        {
            //Arrange
            var model = new CreateAccountModel
            {
                CompanyName = "test",
                UserId = Guid.NewGuid().ToString(),
                EmployerRef = "123ADFC",
                CompanyNumber = "12345",
                CompanyDateOfIncorporation = new DateTime(2016, 10, 30),
                CompanyRegisteredAddress = "My Address",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                SignedAgreement = true,
                UserIsAuthorisedToSign = true
            };

            //Act
            await _employerAccountOrchestrator.CreateAccount(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAccountCommand>(
                        c => c.AccessToken.Equals(model.AccessToken)
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.CompanyName.Equals(model.CompanyName)
                        && c.CompanyNumber.Equals(model.CompanyNumber)
                        && c.CompanyRegisteredAddress.Equals(model.CompanyRegisteredAddress)
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.EmployerRef.Equals(model.EmployerRef)
                        && c.AccessToken.Equals(model.AccessToken)
                        && c.RefreshToken.Equals(model.RefreshToken)
                        && c.SignAgreement.Equals(true)
                    )));
        }

        [Test]
        public async Task ThenIfUserTriesToSignTheAgreementWhenNotAuthorisedTheyGetBadRequest()
        {
            //Arrange
            var model = new CreateAccountModel
            {
                UserIsAuthorisedToSign = false,
                SignedAgreement = true
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetLatestAccountAgreementTemplateRequest>()))
                     .ReturnsAsync(new GetLatestAccountAgreementResponse
                     {
                         Template = new EmployerAgreementTemplate()
                     });

            //Act
            var result = await _employerAccountOrchestrator.CreateAccount(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetLatestAccountAgreementTemplateRequest>()), Times.Once);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }

        [Test]
        public async Task ThenIfUserSignsTheAgreementAndIsAuthorisedToTheyGetOkRequest()
        {
            //Arrange
            var model = new CreateAccountModel
            {
                UserIsAuthorisedToSign = true,
                SignedAgreement = true
            };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetLatestAccountAgreementTemplateRequest>()))
                     .ReturnsAsync(new GetLatestAccountAgreementResponse
                     {
                         Template = new EmployerAgreementTemplate()
                     });

            //Act
            var result = await _employerAccountOrchestrator.CreateAccount(model);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, result.Status);
        }

        [Test]
        public async Task ThenShouldSignAgreementIfUserCanAndHasSignedAgreement()
        {
            //Arrange
            var model = new CreateAccountModel
            {
                CompanyName = "test",
                UserId = Guid.NewGuid().ToString(),
                EmployerRef = "123ADFC",
                CompanyNumber = "12345",
                CompanyDateOfIncorporation = new DateTime(2016, 10, 30),
                CompanyRegisteredAddress = "My Address",
                AccessToken = Guid.NewGuid().ToString(),
                RefreshToken = Guid.NewGuid().ToString(),
                SignedAgreement = true,
                UserIsAuthorisedToSign = true
            };

            //Act
            await _employerAccountOrchestrator.CreateAccount(model);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAccountCommand>(
                        c => c.AccessToken.Equals(model.AccessToken)
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.CompanyName.Equals(model.CompanyName)
                        && c.CompanyNumber.Equals(model.CompanyNumber)
                        && c.CompanyRegisteredAddress.Equals(model.CompanyRegisteredAddress)
                        && c.CompanyDateOfIncorporation.Equals(model.CompanyDateOfIncorporation)
                        && c.EmployerRef.Equals(model.EmployerRef)
                        && c.AccessToken.Equals(model.AccessToken)
                        && c.RefreshToken.Equals(model.RefreshToken)
                        && c.SignAgreement.Equals(true)
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
                    HashedId = hashedId
                });

            //Act
            var response = await _employerAccountOrchestrator.CreateAccount(new CreateAccountModel
            {
                UserIsAuthorisedToSign = true,
                SignedAgreement = true
            });

            //Assert
            Assert.AreEqual(hashedId, response.Data?.EmployerAgreement?.HashedId);

        }
    }
}
