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
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenRemovingAPayeScheme
    {
        private const string SchemeName = "Test Scheme";
        private const string EmpRef = "123/AGB";

        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private EmployerApprenticeshipsServiceConfiguration _configuration;
        private PayeSchemeView _payeScheme;

        [SetUp]
        public void Arrange()
        {
            _payeScheme = new PayeSchemeView
            {
                Ref = EmpRef,
                Name = SchemeName,
                AddedDate = DateTime.Now
            };

            _mediator = new Mock<IMediator>();

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPayeSchemeByRefQuery>()))
                .ReturnsAsync(new GetPayeSchemeByRefResponse
                {
                    PayeScheme = _payeScheme
                });

            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
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
            var model = new RemovePayeSchemeViewModel { HashedAccountId = hashedId,PayeRef = payeRef,UserId = userRef};

            //Act
            await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<RemovePayeFromAccountCommand>(c=>c.HashedAccountId.Equals(hashedId) && c.PayeRef.Equals(payeRef) && c.UserId.Equals(userRef))), Times.Once);
            
        }

        [Test]
        public async Task WhenAnUnathorizedExceptionIsThrownThenAUnauthorizedHttpCodeIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemovePayeFromAccountCommand>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeSchemeViewModel ());

            //Assert
            Assert.AreEqual(actual.Status,HttpStatusCode.Unauthorized);
            
        }

        [Test]
        public async Task WhenAnInvalidRequestExceptionisThrownAndABadRequestHttpCodeIsReturned()
        {

            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<RemovePayeFromAccountCommand>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeSchemeViewModel());

            //Assert
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest);
            
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePayeSchemeWhenASchemeIsSelectedToBeRemoved()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account
                    {
                        Name = "test account"
                    }
                });

            //Act
            await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeSchemeViewModel());


            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetPayeSchemeByRefQuery>()), Times.Once);
        }

        [Test]
        public async Task ThenThePayeSchemeNameShouldBeReturnedWhenTheUserSelectsASchemeToRemove()
        {
            //Arrange
            var hashedId = "ABV465";
            var userRef = "abv345";
            var payeRef = "123/abc";
            var model = new RemovePayeSchemeViewModel { HashedAccountId = hashedId, PayeRef = payeRef, UserId = userRef };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account
                    {
                        Name = "test account"
                    }
                });

            //Act
            var actual = await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(model);

            //Assert
            Assert.AreEqual(SchemeName, actual.Data.PayeSchemeName);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePayeSchemeWhenASchemeIsRemoved()
        {
            //Act
            await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeSchemeViewModel());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<GetPayeSchemeByRefQuery>()), Times.Once);
        }

        [Test]
        public async Task ThenThePayeSchemeNameShouldBeReturnedWhenTheSchemeIsRemoved()
        {
            //Arrange
            var hashedId = "ABV465";
            var userRef = "abv345";
            var payeRef = "123/abc";
            var model = new RemovePayeSchemeViewModel { HashedAccountId = hashedId, PayeRef = payeRef, UserId = userRef };

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            //Assert
            Assert.AreEqual(SchemeName, actual.Data.PayeSchemeName);
        }

    }
}
