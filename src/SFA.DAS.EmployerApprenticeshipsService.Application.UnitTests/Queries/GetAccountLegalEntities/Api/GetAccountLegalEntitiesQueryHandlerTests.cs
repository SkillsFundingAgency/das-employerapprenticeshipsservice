using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Mappings;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.TestCommon;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountLegalEntities.Api
{
    [TestFixture]
    public class GetAccountLegalEntitiesQueryHandlerTests : FluentTest<GetAccountLegalEntitiesQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingGetAccountLegalEntitiesQuery_ThenShouldReturnGetAccountLegalEntitiesResponse()
        {
            return RunAsync(f => f.SetAccountLegalEntities(2), f => f.Handle(), (f, r) =>
            {
                r.Should().NotBeNull().And.BeOfType<GetAccountLegalEntitiesResponse>();
                r.AccountLegalEntities.Should().NotBeNull().And.BeOfType<PagedApiResponseViewModel<AccountLegalEntityViewModel>>();
                r.AccountLegalEntities.Data.ShouldAllBeEquivalentTo(f.AccountLegalEntities.Select(a => new AccountLegalEntityViewModel { AccountLegalEntityId = a.Id, AccountLegalEntityPublicHashedId = a.PublicHashedId }));
                r.AccountLegalEntities.Page.Should().Be(1);
                r.AccountLegalEntities.TotalPages.Should().Be(1);
            });
        }

        [Test]
        public Task Handle_WhenGettingFirstPageOfMultiplePages_ThenShouldReturnFirstPageOfMultiplePages()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(1, 5), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 5 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 2));
        }

        [Test]
        public Task Handle_WhenGettingLastPageOfMultiplePages_ThenShouldReturnLastPageOfMultiplePages()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(4, 3), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 1 &&
                    r2.AccountLegalEntities.Page == 4 &&
                    r2.AccountLegalEntities.TotalPages == 4));
        }

        [Test]
        public Task Handle_WhenGettingSinglePage_ThenShouldReturnSinglePage()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(1, 100), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 10 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 1));
        }

        [Test]
        public Task Handle_WhenGettingNonExistingPage_ThenShouldReturnSinglePage()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(1, 100), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 10 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 1));
        }
    }

    public class GetAccountLegalEntitiesQueryHandlerTestsFixture : FluentTestFixture
    {
        public GetAccountLegalEntitiesQueryHandler Handler { get; set; }
        public GetAccountLegalEntitiesQuery Query { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public Mock<EmployerAccountDbContext> Db { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture()
        {
            Query = new GetAccountLegalEntitiesQuery();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<LegalEntityMappings>();
            });

            Db = new Mock<EmployerAccountDbContext>();
            AccountLegalEntities = new List<AccountLegalEntity>();

            Db.Setup(d => d.AccountLegalEntities).Returns(new DbSetStub<AccountLegalEntity>(AccountLegalEntities));

            Handler = new GetAccountLegalEntitiesQueryHandler(ConfigurationProvider, new Lazy<EmployerAccountDbContext>(() => Db.Object));

            QueryFutureManager.AllowQueryBatch = false;
        }

        public Task<GetAccountLegalEntitiesResponse> Handle()
        {
            return Handler.Handle(Query);
        }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture SetAccountLegalEntities(int count)
        {
            AccountLegalEntities.AddRange(Enumerable.Range(1, count).Select(i => new AccountLegalEntity { Id = i, PublicHashedId = i.ToString() }));

            return this;
        }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture SetQuery(int pageNumber, int pageSize)
        {
            Query.PageNumber = pageNumber;
            Query.PageSize = pageSize;

            return this;
        }
    }
}
