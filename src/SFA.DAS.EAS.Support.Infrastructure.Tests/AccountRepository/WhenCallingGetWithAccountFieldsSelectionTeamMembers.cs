using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldsSelectionTeamMembers : WhenTestingAccountRepository
    {
        [Test]
        public async Task
            ItShouldReturnTheMatchingAccountWithAnEmptyListOfTeamMembersWhenAnExceptionIsThrownObtainingTheTeeamMembers()
        {
            var id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                HashedAccountId = "ASDAS",
                AccountId = 123
            };
            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var teamMemberResponse = new List<TeamMemberViewModel>
            {
                new TeamMemberViewModel {Email = "member.1.@tempuri.org"},
                new TeamMemberViewModel {Email = "member.1.@tempuri.org"}
            };
            AccountApiClient.Setup(x => x.GetAccountUsers(accountDetailViewModel.HashedAccountId))
                .ThrowsAsync(new Exception("Some Exception"));

            var actual = await _sut.Get(id, AccountFieldsSelection.TeamMembers);


            Logger.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Exactly(2));


            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.TeamMembers);
            CollectionAssert.IsEmpty(actual.TeamMembers);
            Assert.IsNull(actual.Transactions);
            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.LegalEntities);
        }

        [Test]
        public async Task ItShouldReturnTheMatchingAccountWithTeamMembers()
        {
            var id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                HashedAccountId = "ASDAS",
                AccountId = 123
            };
            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var teamMemberResponse = new List<TeamMemberViewModel>
            {
                new TeamMemberViewModel {Email = "member.1.@tempuri.org"},
                new TeamMemberViewModel {Email = "member.1.@tempuri.org"}
            };
            AccountApiClient.Setup(x => x.GetAccountUsers(accountDetailViewModel.HashedAccountId))
                .ReturnsAsync(teamMemberResponse);

            var actual = await _sut.Get(id, AccountFieldsSelection.TeamMembers);


            Logger.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Exactly(2));


            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.TeamMembers);
            Assert.AreEqual(teamMemberResponse.Count, actual.TeamMembers.Count);
            Assert.IsNull(actual.Transactions);
            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.LegalEntities);
        }
    }
}