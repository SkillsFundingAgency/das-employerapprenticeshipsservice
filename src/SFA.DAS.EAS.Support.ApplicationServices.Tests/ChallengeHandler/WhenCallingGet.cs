using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Support.ApplicationServices.Tests.ChallengeHandler
{
    [TestFixture]
    public class WhenCallingGet : WhenTestingChallengeHandler
    {
        [Test]
        public async Task ItShouldReturnAnAccountAndSuccessWhenQueryHasAMatch()
        {
            const string id = "123";
            var account = new Core.Models.Account
            {
                HashedAccountId = "ASDAS",
                AccountId = 123
            };
            _accountRepository.Setup(x =>
                    x.Get(id,
                        AccountFieldsSelection.PayeSchemes))
                .ReturnsAsync(account);

            var actual = await _unit.Get(id);


            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Account);
            Assert.AreEqual(SearchResponseCodes.Success, actual.StatusCode);
        }

        [Test]
        public async Task ItShouldReturnNoSearchResultsFoundWhenQueryHasNoMatch()
        {
            const string id = "123";
            _accountRepository.Setup(x =>
                    x.Get(id,
                        AccountFieldsSelection.PayeSchemes))
                .ReturnsAsync(null as Core.Models.Account);

            var actual = await _unit.Get(id);

            Assert.IsNotNull(actual);
            Assert.AreEqual(SearchResponseCodes.NoSearchResultsFound, actual.StatusCode);
        }
    }
}