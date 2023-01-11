using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;
using SFA.DAS.EmployerAccounts.TestCommon;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.AccountLegalEntitiesControllerTests
{
    [TestFixture]
    public class AccountLegalEntitiesControllerTests : FluentTest<AccountLegalEntitiesControllerTestsFixture>
    {
        [Test]
        public Task Get_WhenGettingAccountLegalEntities_ThenShouldReturnAccountLegalEntities()
        {
            return RunAsync(f => f.Get(), (f, r) => r.Should().NotBeNull().And.Match<OkNegotiatedContentResult<PagedApiResponse<AccountLegalEntity>>>(o => o.Content == f.AccountLegalEntities));
        }
    }

    public class AccountLegalEntitiesControllerTestsFixture : FluentTestFixture
    {
        public AccountLegalEntitiesController Controller { get; set; }
        public GetAccountLegalEntitiesQuery Query { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public GetAccountLegalEntitiesResponse Response { get; set; }
        public PagedApiResponse<AccountLegalEntity> AccountLegalEntities { get; set; }

        public AccountLegalEntitiesControllerTestsFixture()
        {
            Query = new GetAccountLegalEntitiesQuery { PageSize = 1000, PageNumber = 1 };
            Mediator = new Mock<IMediator>();
            AccountLegalEntities = new PagedApiResponse<AccountLegalEntity>();
            Response = new GetAccountLegalEntitiesResponse { AccountLegalEntities = AccountLegalEntities };

            Mediator.Setup(m => m.SendAsync(Query)).ReturnsAsync(Response);

            Controller = new AccountLegalEntitiesController(Mediator.Object);
        }

        public Task<IHttpActionResult> Get()
        {
            return Controller.Get(Query);
        }
    }
}