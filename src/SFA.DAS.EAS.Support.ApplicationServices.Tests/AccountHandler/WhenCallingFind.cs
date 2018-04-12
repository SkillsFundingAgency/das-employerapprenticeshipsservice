using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenCallingFind : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnAccountInResponseIfFound()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};


            MockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.None)).ReturnsAsync(account);

            var actual = await Unit.Find(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
            Assert.IsNotNull(actual.Account);
        }

        [Test]
        public async Task ItShouldReturnNoAccountInTheResponseIfNotFound()
        {
            var accountId = 123L;
            var orgid = accountId.ToString();
            var account = new Core.Models.Account {AccountId = accountId};


            MockAccountRepository.Setup(r => r.Get(orgid, AccountFieldsSelection.None))
                .ReturnsAsync(null as Core.Models.Account);

            var actual = await Unit.Find(orgid);
            Assert.IsNotNull(actual);
            Assert.AreEqual(new AccountDetailOrganisationsResponse().StatusCode, actual.StatusCode);
            Assert.IsNull(actual.Account);
        }
    }
}