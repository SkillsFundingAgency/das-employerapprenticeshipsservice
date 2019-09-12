using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    [TestFixture]
    public class WhenIGetPayeSchemesForAnAccount : AccountPayeSchemesControllerTests
    {
        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            ApiService.Setup(x => x.GetAccount(hashedAccountId, It.IsAny<CancellationToken>())).ReturnsAsync(new AccountDetailViewModel());

            var response = await Controller.GetPayeSchemes(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
