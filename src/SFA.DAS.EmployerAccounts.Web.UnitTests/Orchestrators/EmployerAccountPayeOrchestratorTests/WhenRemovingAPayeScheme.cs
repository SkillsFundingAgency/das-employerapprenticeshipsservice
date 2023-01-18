using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountPayeOrchestratorTests
{
    public class WhenRemovingAPayeScheme
    {
        private const string SchemeName = "Test Scheme";
        private const string EmpRef = "123/AGB";

        private EmployerAccountPayeOrchestrator _employerAccountPayeOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private EmployerAccountsConfiguration _configuration;
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

            _mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeByRefQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetPayeSchemeByRefResponse
                {
                    PayeScheme = _payeScheme
                });

            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _configuration = new EmployerAccountsConfiguration();
            _employerAccountPayeOrchestrator = new EmployerAccountPayeOrchestrator(_mediator.Object, _cookieService.Object, _configuration);
        }

        [Test]
        public async Task ThenTheCommandIsCalledForRemovingThePayeScheme()
        {
            //Arrange
            var hashedId = "ABV465";
            var userRef = "abv345";
            var payeRef = "123/abc";

            var model = new RemovePayeSchemeViewModel
            {
                HashedAccountId = hashedId,
                PayeRef = payeRef,
                UserId = userRef
            };

            //Act
            await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(model);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<RemovePayeFromAccountCommand>(c => c.HashedAccountId.Equals(hashedId) && c.PayeRef.Equals(payeRef) && c.UserId.Equals(userRef) && c.CompanyName.Equals(SchemeName)), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Test]
        public async Task WhenAnUnathorizedExceptionIsThrownThenAUnauthorizedHttpCodeIsReturned()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<RemovePayeFromAccountCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new UnauthorizedAccessException(""));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeSchemeViewModel());

            //Assert
            Assert.AreEqual(actual.Status, HttpStatusCode.Unauthorized);

        }

        [Test]
        public async Task WhenAnInvalidRequestExceptionisThrownAndABadRequestHttpCodeIsReturned()
        {

            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<RemovePayeFromAccountCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act
            var actual = await _employerAccountPayeOrchestrator.RemoveSchemeFromAccount(new RemovePayeSchemeViewModel());

            //Assert
            Assert.AreEqual(actual.Status, HttpStatusCode.BadRequest);

        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePayeSchemeWhenASchemeIsSelectedToBeRemoved()
        {
            //Arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEmployerAccountByHashedIdResponse
                {
                    Account = new Account
                    {
                        Name = "test account"
                    }
                });

            //Act
            await _employerAccountPayeOrchestrator.GetRemovePayeSchemeModel(new RemovePayeSchemeViewModel());


            //Assert
            _mediator.Verify(x => x.Send(It.IsAny<GetPayeSchemeByRefQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ThenThePayeSchemeNameShouldBeReturnedWhenTheUserSelectsASchemeToRemove()
        {
            //Arrange
            var hashedId = "ABV465";
            var userRef = "abv345";
            var payeRef = "123/abc";
            var model = new RemovePayeSchemeViewModel { HashedAccountId = hashedId, PayeRef = payeRef, UserId = userRef };

            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetEmployerAccountByHashedIdResponse
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
            _mediator.Verify(x => x.Send(It.IsAny<GetPayeSchemeByRefQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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
