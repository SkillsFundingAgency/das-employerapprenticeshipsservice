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
            const string id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                HashedAccountId = "ASDAS",
                AccountId = 123
            };
            AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            AccountApiClient.Setup(x => x.GetAccountUsers(accountDetailViewModel.HashedAccountId))
                .ThrowsAsync(new Exception("Some Exception"));

            var actual = await Sut!.Get(id, AccountFieldsSelection.TeamMembers);

            Logger!.Verify(
                x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.TeamMembers, Is.Not.Null);
                CollectionAssert.IsEmpty(actual.TeamMembers);
                Assert.That(actual.Transactions, Is.Null);
                Assert.That(actual.PayeSchemes, Is.Null);
                Assert.That(actual.LegalEntities, Is.Null);
            });
        }

        [Test]
        public async Task ItShouldReturnTheMatchingAccountWithTeamMembers()
        {
            const string id = "123";

            var accountDetailViewModel = new AccountDetailViewModel
            {
                HashedAccountId = "ASDAS",
                AccountId = 123
            };

            AccountApiClient!.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(accountDetailViewModel);

            var teamMemberResponse = new List<TeamMemberViewModel>
            {
                new() { Email = "member.1.@tempuri.org" },
                new() { Email = "member.1.@tempuri.org" }
            };
            AccountApiClient.Setup(x => x.GetAccountUsers(accountDetailViewModel.HashedAccountId))
                .ReturnsAsync(teamMemberResponse);

            var actual = await Sut!.Get(id, AccountFieldsSelection.TeamMembers);

            Logger!.Verify(
                x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));

            Assert.Multiple(() =>
            {
                Assert.That(actual, Is.Not.Null);
                Assert.That(actual.TeamMembers, Is.Not.Null);
                Assert.That(actual.TeamMembers, Has.Count.EqualTo(teamMemberResponse.Count));
                Assert.That(actual.Transactions, Is.Null);
                Assert.That(actual.PayeSchemes, Is.Null);
                Assert.That(actual.LegalEntities, Is.Null);
            });
        }
    }
}