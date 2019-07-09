using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;
using StructureMap;

namespace SFA.DAS.EAS.Portal.UnitTests.Client
{
    [TestFixture, Parallelizable, Ignore("WIP")]
    public class PortalClientTests : FluentTest<PortalClientTestsFixture>
    {
        //todo: account related tests, ie. account returned, account not returned
        
        [Test]
        public Task GetAccount_WhenHasNoPayeScheme_ThenVacancyCardinalityIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasNoPayeScheme_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQueryFails_ThenVacancyCardinalityIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQueryFails_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsNoVacancies_ThenVacancyCardinalityIsSetToNone()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsNoVacancies_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }
        
        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsOneVacancies_ThenVacancyCardinalityIsSetToOne()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsOneVacancies_ThenSingleVacancyIsSetCorrectly()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsTwoVacancies_ThenVacancyCardinalityIsSetToMany()
        {
            return TestAsync(f => f.GetAccount());
        }

        [Test]
        public Task GetAccount_WhenHasPayeSchemeAndQuerySucceedsAndReturnsTwoVacancies_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.GetAccount());
        }
    }

    public class PortalClientTestsFixture
    {
        PortalClient PortalClient { get; }
        Mock<IContainer> MockContainer { get; } = new Mock<IContainer>();
        private Mock<IGetAccountQuery> MockGetAccountQuery { get; } = new Mock<IGetAccountQuery>();
        Mock<IDasRecruitService> MockDasRecruitService { get; } = new Mock<IDasRecruitService>();
        Account Account { get; }
        IEnumerable<Vacancy> Vacancies { get; }
        const long AccountId = 999L;
        
        public PortalClientTestsFixture()
        {
            MockGetAccountQuery.Setup(q => q.Get(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Account);
            MockDasRecruitService.Setup(s => s.GetVacancies(AccountId, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Vacancies);
            
            MockContainer.Setup(c => c.GetInstance<IGetAccountQuery>())
                .Returns(MockGetAccountQuery.Object);
            MockContainer.Setup(c => c.GetInstance<IDasRecruitService>())
                .Returns(MockDasRecruitService.Object);
            
            PortalClient = new PortalClient(MockContainer.Object);
        }

        public PortalClientTestsFixture ArrangeQueryFails()
        {
            return this;
        }

        public PortalClientTestsFixture ArrangeQuerySucceedsAndReturnsNoVacancies()
        {
            return this;
        }
        
        public PortalClientTestsFixture ArrangeQuerySucceedsAndReturnsOneVacancy()
        {
            return this;
        }

        public PortalClientTestsFixture ArrangeQuerySucceedsAndReturnsTwoVacancies()
        {
            return this;
        }

        public Account GetAccount()
        {
            throw new NotImplementedException();
        }
    }
}