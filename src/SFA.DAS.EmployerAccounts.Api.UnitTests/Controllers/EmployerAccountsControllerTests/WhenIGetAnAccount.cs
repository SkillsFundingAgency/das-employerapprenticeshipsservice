using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public class WhenIGetAnAccount : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountIsReturned()
        {
            var hashedAccountId = "ABC123";

            var accountsResponse = new GetEmployerAccountDetailByHashedIdResponse
            {
                Account = new Models.Account.AccountDetail()
                {
                    AccountId = 123,
                    HashedId = hashedAccountId,
                    Name = "Test 1"
                }
            };

            Mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(accountsResponse);
          
            var response = await Controller.GetAccount(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountDetail>>(response);
            var model = response as OkNegotiatedContentResult<AccountDetail>;

            model?.Content?.Should().NotBeNull();
            model?.Content?.AccountId.Should().Be(123);
            model?.Content?.DasAccountName.Should().Be("Test 1");
            model?.Content?.HashedAccountId.Should().Be(hashedAccountId);
        }
    }
}
