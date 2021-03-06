﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldSelectionTeamMembers : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnTheAccountAndTheTeamMembers()
        {
            var id = "123";

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(new AccountDetailViewModel()
                {

                } );

            var fixture = new Fixture();
            AccountApiClient.Setup(x => x.GetAccountUsers( It.IsAny<string>())).ReturnsAsync(new List<TeamMemberViewModel>()
            {
                fixture.Create<TeamMemberViewModel>(),
                fixture.Create<TeamMemberViewModel>()
            } );

            var actual = await _sut.Get(id, AccountFieldsSelection.TeamMembers);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Exactly(2));

            Assert.IsNotNull(actual);
            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.LegalEntities);
            Assert.IsNotEmpty(actual.TeamMembers);
            Assert.IsNull(actual.Transactions);
        }

        [Test]
        public async Task ItShouldReturnTheAccountWithEmptyTeamMembersOnException()
        {
            var id = "123";

            
            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ThrowsAsync(new Exception());

            var actual = await _sut.Get(id, AccountFieldsSelection.TeamMembers);

            Logger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);
            Logger.Verify(x => x.Error(It.IsAny<Exception>(), $"Account with id {id} not found"));

            Assert.IsNull(actual);
           
        }
    }
}