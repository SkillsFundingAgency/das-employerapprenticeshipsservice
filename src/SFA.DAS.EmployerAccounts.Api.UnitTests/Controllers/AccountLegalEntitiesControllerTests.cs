using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.LegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetPagedAccountLegalEntities;
using SFA.DAS.Testing;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers
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
        public GetPagedAccountLegalEntitiesQuery Query { get; set; }
        public Mock<IMediator> Mediator { get; set; }
        public GetPagedAccountLegalEntitiesResponse Response { get; set; }
        public PagedApiResponseViewModel<AccountLegalEntityViewModel> AccountLegalEntities { get; set; }

        public AccountLegalEntitiesControllerTestsFixture()
        {
            Query = new GetPagedAccountLegalEntitiesQuery { PageSize = 1000, PageNumber = 1 };
            Mediator = new Mock<IMediator>();
            AccountLegalEntities = new PagedApiResponseViewModel<AccountLegalEntityViewModel>();
            Response = new GetPagedAccountLegalEntitiesResponse { AccountLegalEntities = AccountLegalEntities };

            Mediator.Setup(m => m.SendAsync(Query)).ReturnsAsync(Response);

            Controller = new AccountLegalEntitiesController(Mediator.Object);
        }

        public Task<IHttpActionResult> Get()
        {
            return Controller.Get(Query);
        }
    }
}