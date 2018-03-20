using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.AccountHandler
{
    [TestFixture]
    public class WhenCallingFindSearchItems : WhenTestingAccountHandler
    {
        [Test]
        public async Task ItShouldReturnAnAccountForEachItemInResponseIfFound()
        {
            var accountDetailModels = new List<Core.Models.Account>
            {
                new Core.Models.Account
                {
                    AccountId = 123,
                    OwnerEmail = "owner1@tempuri.org",
                    HashedAccountId = "ABC78"
                },
                new Core.Models.Account
                {
                    AccountId = 124,
                    OwnerEmail = "owner2@tempuri.org",
                    HashedAccountId = "DEF12"
                }
            };

            MockAccountRepository.Setup(r => r.FindAllDetails(10,1)).ReturnsAsync(accountDetailModels);

            var actual = await Unit.FindAllAccounts(10,1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public async Task ItShouldReturnAnEmptyCollectionIfNotFound()
        {
            var accountDetailModels = new List<Core.Models.Account>();

            MockAccountRepository.Setup(r => r.FindAllDetails(10,1)).ReturnsAsync(accountDetailModels);

            var actual = await Unit.FindAllAccounts(10,1);
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }
    }
}