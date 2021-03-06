﻿using System;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests.CreateAccountForSkipJourney.Given_User_Does_Not_Already_Have_An_Account
{
    public class WhenCreatingTheUserAccount
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private EmployerAccountsConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _configuration = new EmployerAccountsConfiguration();

            _employerAccountOrchestrator = new EmployerAccountOrchestrator(
                _mediator.Object, 
                _logger.Object,
                _cookieService.Object, 
                _configuration);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserAccountsQuery>()))
                .ReturnsAsync(new GetUserAccountsQueryResponse()
                {
                    Accounts = new Accounts<Account>()
                });


            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateUserAccountCommand>()))
                .ReturnsAsync(new CreateUserAccountCommandResponse()
                {
                    HashedAccountId = "ABS10"
                });
        }

        [Test]
        public async Task ThenTheUserAccountIsCreatedWithTheCorrectValues()
        {
            //Arrange
            var model = ArrangeModel();

            //Act
            await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(model, It.IsAny<HttpContextBase>());

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateUserAccountCommand>(
                c => c.OrganisationName.Equals(model.OrganisationName)
            )));
        }

        [Test]
        public async Task ThenIShouldGetBackTheNewAccountId()
        {
            //Assign
            const string hashedId = "1AFGG0";

            _mediator.Setup(x => x.SendAsync(It.IsAny<CreateUserAccountCommand>()))
                .ReturnsAsync(new CreateUserAccountCommandResponse()
                {
                    HashedAccountId = hashedId
                });

            //Act
            var response =
                await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(new CreateUserAccountViewModel(),
                    It.IsAny<HttpContextBase>());

            //Assert
            Assert.AreEqual(hashedId, response.Data?.HashedId);
        }

        private static CreateUserAccountViewModel ArrangeModel()
        {
            return new CreateUserAccountViewModel
            {
                OrganisationName = "test",
                UserId = Guid.NewGuid().ToString()
            };
        }
    }
}

