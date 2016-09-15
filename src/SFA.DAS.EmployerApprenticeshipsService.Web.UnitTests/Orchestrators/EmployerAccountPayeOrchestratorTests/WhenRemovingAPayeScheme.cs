﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenRemovingAPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieService> _cookieService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private Mock<IEmpRefFileBasedService> _empRefFileBasedService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieService>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();
            _empRefFileBasedService = new Mock<IEmpRefFileBasedService>();
            
            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object,_cookieService.Object,_configuration,_empRefFileBasedService.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCalledForRemovingThePayeScheme()
        {
            //Arrange
            long accountId = 1234567;
            var userRef = "abv345";
            var payeRef = "123/abc";
            var model = new RemovePayeScheme {AccountId = accountId,PayeRef = payeRef,UserId = userRef};

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<RemovePayeFromAccountCommand>(c=>c.AccountId.Equals(accountId) && c.PayeRef.Equals(payeRef) && c.UserId.Equals(userRef))), Times.Once);
            Assert.IsTrue(actual.Data);
        }

        [Test]
        public async Task WhenAnUnathorizedExceptionIsThrownThenAUnauthorizedHttpCodeIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemovePayeFromAccountCommand>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeScheme ());

            //Assert
            Assert.AreEqual(actual.Status,HttpStatusCode.Unauthorized);
            Assert.IsFalse(actual.Data);
        }

        [Test]
        public async Task WhenAnInvalidRequestExceptionisThrownAndABadRequestHttpCodeIsReturned()
        {

            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemovePayeFromAccountCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeScheme());

            //Assert
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest);
            Assert.IsFalse(actual.Data);
        }
        
    }
}
