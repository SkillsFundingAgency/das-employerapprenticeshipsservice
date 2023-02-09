using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities.Api;
using SFA.DAS.EmployerAccounts.TestCommon;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntities.Api
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
                r.AccountLegalEntities.Should().NotBeNull().And.BeOfType<PagedApiResponse<AccountLegalEntity>>();

                var expected = f.AccountLegalEntities.Select(a => new AccountLegalEntity { AccountLegalEntityId = a.Id, AccountLegalEntityPublicHashedId = a.PublicHashedId });
                r.AccountLegalEntities.Data.Should().BeEquivalentTo(expected);

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
        public IMapper Mapper { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public List<EmployerAccounts.Models.Account.AccountLegalEntity> AccountLegalEntities { get; set; }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture()
        {
            Query = new GetAccountLegalEntitiesQuery();

            Mapper = new Mapper(new MapperConfiguration(c =>
            {
                c.AddProfile<LegalEntityMappings>();
            }));

            Db = new Mock<EmployerAccountsDbContext>();
            AccountLegalEntities = new List<EmployerAccounts.Models.Account.AccountLegalEntity>();
           
            var mockDbSet = AccountLegalEntities.AsQueryable().BuildMockDbSet();

            Db.Setup(d => d.AccountLegalEntities).Returns(mockDbSet.Object);

            Handler = new GetAccountLegalEntitiesQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), Mapper);
        }

        public Task<GetAccountLegalEntitiesResponse> Handle()
        {
            return Handler.Handle(Query, CancellationToken.None);
        }

        public GetAccountLegalEntitiesQueryHandlerTestsFixture SetAccountLegalEntities(int count)
        {
            AccountLegalEntities.AddRange(Enumerable.Range(1, count).Select(i => new EmployerAccounts.Models.Account.AccountLegalEntity { Id = i, PublicHashedId = i.ToString() }));

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
