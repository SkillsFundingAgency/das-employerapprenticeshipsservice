using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenRemovingAPayeScheme
    {
        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
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
            new Mock<IEmpRefFileBasedService>();
            
            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _logger.Object,_cookieService.Object,_configuration);
        }

        [Test]
        public async Task ThenTheCommandIsCalledForRemovingThePayeScheme()
        {
            //Arrange
            var hashedId = "ABV465";
            var userRef = "abv345";
            var payeRef = "123/abc";
            var model = new RemovePayeScheme { AccountHashedId = hashedId,PayeRef = payeRef,UserId = userRef};

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<RemovePayeFromAccountCommand>(c=>c.HashedId.Equals(hashedId) && c.PayeRef.Equals(payeRef) && c.UserId.Equals(userRef))), Times.Once);
            
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
            
        }
        
    }
}
