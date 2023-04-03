using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.AccountRepository
{
    [TestFixture]
    public class WhenCallingGetWithAccountFieldSelectionNone : WhenTestingAccountRepository
    {
        [Test]
        public async Task ItShouldReturnJustTheAccount()
        {
            var id = "123";

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ReturnsAsync(new AccountDetailViewModel());

            var actual = await _sut.Get(id, AccountFieldsSelection.None);

            Logger.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Once);

            Assert.IsNotNull(actual);
            Assert.IsNull(actual.PayeSchemes);
            Assert.IsNull(actual.LegalEntities);
            Assert.IsNull(actual.TeamMembers);
            Assert.IsNull(actual.Transactions);
        }

        [Test]
        public async Task ItShouldReturnNullOnException()
        {
            var id = "123";

            AccountApiClient.Setup(x => x.GetResource<AccountDetailViewModel>($"/api/accounts/{id}"))
                .ThrowsAsync(new Exception());

            var actual = await _sut.Get(id, AccountFieldsSelection.None);

            Logger.Verify(x => x.LogDebug(It.IsAny<string>()), Times.Once);
            Logger.Verify(x => x.LogError(It.IsAny<Exception>(), $"Account with id {id} not found"));

            Assert.IsNull(actual);
        }
    }
}