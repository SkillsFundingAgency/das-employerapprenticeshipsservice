using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.LegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetPagedAccountLegalEntities;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;
using Z.EntityFramework.Plus;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetPagedAccountLegalEntities
{
    [TestFixture]
    public class GetAccountLegalEntitiesQueryHandlerTests : FluentTest<GetAccountLegalEntitiesQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingGetAccountLegalEntitiesQuery_ThenShouldReturnGetAccountLegalEntitiesResponse()
        {
            return RunAsync(f => f.SetAccountLegalEntities(2), f => f.Handle(), (f, r) =>
            {
                r.Should().NotBeNull().And.BeOfType<GetPagedAccountLegalEntitiesResponse>();
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
                .And.Match<GetPagedAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 5 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 2));
        }

        [Test]
        public Task Handle_WhenGettingLastPageOfMultiplePages_ThenShouldReturnLastPageOfMultiplePages()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(4, 3), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetPagedAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 1 &&
                    r2.AccountLegalEntities.Page == 4 &&
                    r2.AccountLegalEntities.TotalPages == 4));
        }

        [Test]
        public Task Handle_WhenGettingSinglePage_ThenShouldReturnSinglePage()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(1, 100), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetPagedAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 10 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 1));
        }

        [Test]
        public Task Handle_WhenGettingNonExistingPage_ThenShouldReturnSinglePage()
        {
            return RunAsync(f => f.SetAccountLegalEntities(10).SetQuery(1, 100), f => f.Handle(), (f, r) => r.Should().NotBeNull()
                .And.Match<GetPagedAccountLegalEntitiesResponse>(r2 =>
                    r2.AccountLegalEntities.Data.Count == 10 &&
                    r2.AccountLegalEntities.Page == 1 &&
                    r2.AccountLegalEntities.TotalPages == 1));
        }
    }

    public class GetAccountLegalEntitiesQueryHandlerTestsFixture : FluentTestFixture
    {
        public GetPagedAccountLegalEntitiesQueryHandler Handler { get; set; }
        public GetPagedAccountLegalEntitiesQuery Query { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public List<AccountLegalEntity> AccountLegalEntities { get; set; }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture()
        {
            Query = new GetPagedAccountLegalEntitiesQuery();

            ConfigurationProvider = new MapperConfiguration(c =>
            {
                c.AddProfile<LegalEntityMappings>();
            });

            Db = new Mock<EmployerAccountsDbContext>();
            AccountLegalEntities = new List<AccountLegalEntity>();

            Db.Setup(d => d.AccountLegalEntities).Returns(new DbSetStub<AccountLegalEntity>(AccountLegalEntities));

            Handler = new GetPagedAccountLegalEntitiesQueryHandler(ConfigurationProvider, new Lazy<EmployerAccountsDbContext>(() => Db.Object));

            QueryFutureManager.AllowQueryBatch = false;
        }

        public Task<GetPagedAccountLegalEntitiesResponse> Handle()
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
