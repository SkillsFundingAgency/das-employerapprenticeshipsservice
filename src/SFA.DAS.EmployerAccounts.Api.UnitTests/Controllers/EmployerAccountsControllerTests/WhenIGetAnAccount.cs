using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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

            Mediator.Setup(x => x.Send(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountsResponse);

            var response = await Controller.GetAccount(hashedAccountId) as OkObjectResult;

            Assert.IsNotNull(response);
            var model = response.Value as AccountDetail;

            Assert.IsInstanceOf<AccountDetail>(model);
            model.Should().NotBeNull();
            model.AccountId.Should().Be(123);
            model.DasAccountName.Should().Be("Test 1");
            model.HashedAccountId.Should().Be(hashedAccountId);
        }
    }
}
