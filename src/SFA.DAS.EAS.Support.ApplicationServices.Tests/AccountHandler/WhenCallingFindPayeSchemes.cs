using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenCallingFindPayeSchemes : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnAccountInResponseIfFound()
        {
            const long accountId = 123L;
            var originalId = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};

            MockAccountRepository.Setup(r => r.Get(originalId, AccountFieldsSelection.PayeSchemes))
                .ReturnsAsync(account);

            var actual = await Unit.FindPayeSchemes(originalId);
            Assert.That(actual, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(actual.StatusCode, Is.EqualTo(SearchResponseCodes.Success));
                Assert.That(actual.Account, Is.Not.Null);
            });
        }

        [Test]
        public async Task ItShouldReturnNoAccountInTheResponseIfNotFound()
        {
            const long accountId = 123L;
            var originalId = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};


            MockAccountRepository.Setup(r => r.Get(originalId, AccountFieldsSelection.PayeSchemes))
                .ReturnsAsync(null as Core.Models.Account);

            var actual = await Unit.FindPayeSchemes(originalId);
            Assert.That(actual, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(actual.StatusCode, Is.EqualTo(new AccountDetailOrganisationsResponse().StatusCode));
                Assert.That(actual.Account, Is.Null);
            });
        }
    }
}