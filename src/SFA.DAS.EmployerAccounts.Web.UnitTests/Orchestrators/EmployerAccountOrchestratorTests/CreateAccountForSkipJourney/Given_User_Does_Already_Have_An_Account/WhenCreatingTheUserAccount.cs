﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests.CreateAccountForSkipJourney.Given_User_Does_Already_Have_An_Account
{
    public class WhenCreatingTheUserAccount
    {
        private EmployerAccountOrchestrator _employerAccountOrchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private EmployerAccountsConfiguration _configuration;
        private string _existingAccountHashedId = "B3AE4BBE-7E75-4FF3-89B8-3C24FDFCFE10";

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
                    Accounts = new Accounts<Account> { AccountsCount = 1, AccountList = new List<Account>{ new Account { HashedId = _existingAccountHashedId } } }
                });
        }

        [Test]
        public async Task ThenNewAccountIsNotCreated()
        {
            await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(
                ArrangeModel(),
                It.IsAny<HttpContextBase>());

            _mediator.Verify(
                x => x.SendAsync<CreateUserAccountCommandResponse>(
                    It.IsAny<CreateUserAccountCommand>()),
                Times.Never());
        }

        [Test]
        public async Task ThenResponseContainsHashedIdOfExistingAccount()
        {
            var response =
                await _employerAccountOrchestrator.CreateMinimalUserAccountForSkipJourney(new CreateUserAccountViewModel(),
                    It.IsAny<HttpContextBase>());

            Assert.AreEqual(_existingAccountHashedId, response.Data?.HashedId);
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

    public class TestUser : User
    {
        public TestUser(ICollection<Membership> injectedMemberships)
        {
            Memberships = injectedMemberships;
        }
    }
}

