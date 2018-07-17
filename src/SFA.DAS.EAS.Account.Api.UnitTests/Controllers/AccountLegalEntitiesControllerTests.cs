using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers
{
    [TestFixture]
    public class AccountLegalEntitiesControllerTests : FluentTest<AccountLegalEntitiesControllerTestsFixture>
    {
        [Test]
        public Task Get_WhenGettingAccountLegalEntities_ThenShouldReturnAccountLegalEntities()
        {
            return RunAsync(f => f.Get(), (f, r) => r.Should().NotBeNull().And.Match<OkNegotiatedContentResult<PagedApiResponseViewModel<AccountLegalEntityViewModel>>>(o => o.Content == f.AccountLegalEntities));
        }
    }

    public class AccountLegalEntitiesControllerTestsFixture : FluentTestFixture
    {
        public AccountLegalEntitiesController Controller { get; set; }
        public GetAccountLegalEntitiesQuery Query { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public GetAccountLegalEntitiesResponse Response { get; set; }
        public PagedApiResponseViewModel<AccountLegalEntityViewModel> AccountLegalEntities { get; set; }

        public AccountLegalEntitiesControllerTestsFixture()
        {
            Query = new GetAccountLegalEntitiesQuery { PageSize = 1000, PageNumber = 1 };
            Mediator = new Mock<IMediator>();
            AccountLegalEntities = new PagedApiResponseViewModel<AccountLegalEntityViewModel>();
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