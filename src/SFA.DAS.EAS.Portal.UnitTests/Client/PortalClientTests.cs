using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client;
using SFA.DAS.EAS.Portal.Client.Application.Queries;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Encoding;
using SFA.DAS.Testing;
using StructureMap;

namespace SFA.DAS.EAS.Portal.UnitTests.Client
{
    /// <summary>
    /// These tests don't cover every permutation, only a (sensible) subset
    /// </summary>
    [TestFixture, Parallelizable]
    public class PortalClientTests : FluentTest<PortalClientTestsFixture>
    {
        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndHasNoPayeScheme_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeHasNoPayeScheme(),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasNoPayeScheme_ThenVacancyCardinalityIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasNoPayeScheme(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacancyCardinalityIsNotSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasNoPayeScheme_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasNoPayeScheme(),
                f => f.GetAccount(), (f, r) => f.AssertSingleVacancyIsNotSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallFails_ThenVacancyCardinalityIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacancyCardinalityIsNotSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallFails_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertSingleVacancyIsNotSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenVacancyCardinalityIsSetToNone()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.None));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertSingleVacancyIsNotSet(r));
        }
        
        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenVacancyCardinalityIsSetToOne()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.One));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenSingleVacancyIsSetCorrectly()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertSingleVacancyIsSetCorrectly(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenVacancyCardinalityIsSetToMany()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsTwoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.Many));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenSingleVacancyIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsTwoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertSingleVacancyIsNotSet(r));
        }
    }

    public class PortalClientTestsFixture
    {
        PortalClient PortalClient { get; set; }
        Mock<IContainer> MockContainer { get; set; } = new Mock<IContainer>();
        private Mock<IEncodingService> MockEncodingService { get; set; } = new Mock<IEncodingService>();
        Mock<IGetAccountQuery> MockGetAccountQuery { get; set; } = new Mock<IGetAccountQuery>();
        Mock<IDasRecruitService> MockDasRecruitService { get; set; } = new Mock<IDasRecruitService>();
        AccountState AccountState { get; set; }
        Account Account { get; set; }
        Vacancy Vacancy { get; set; }
        Vacancy OriginalVacancy { get; set; }
        IEnumerable<Vacancy> Vacancies { get; set; }
        const long AccountId = 999L;
        const string PublicHashedAccountId = "HASH12";

        public PortalClientTestsFixture()
        {
            Vacancy = new Fixture().Create<Vacancy>();
            
            Account = JsonConvert.DeserializeObject<Account>($"{{\"Id\": {AccountId} }}");

            MockContainer.Setup(c => c.GetInstance<IGetAccountQuery>())
                .Returns(MockGetAccountQuery.Object);
            MockContainer.Setup(c => c.GetInstance<IDasRecruitService>())
                .Returns(MockDasRecruitService.Object);

            MockEncodingService.Setup(s => s.Decode(PublicHashedAccountId, EncodingType.PublicAccountId))
                .Returns(AccountId);
            
            PortalClient = new PortalClient(MockContainer.Object, MockEncodingService.Object);
        }

        public PortalClientTestsFixture ArrangeAccountExists()
        {
            MockGetAccountQuery.Setup(q => q.Get(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Account);
            
            return this;
        }

        public PortalClientTestsFixture ArrangeAccountDoesNotExist()
        {
            MockGetAccountQuery.Setup(q => q.Get(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(Account));
            
            return this;
        }

        public PortalClientTestsFixture ArrangeHasPayeScheme()
        {
            AccountState |= AccountState.HasPayeScheme;
            
            return this;
        }
        
        public PortalClientTestsFixture ArrangeHasNoPayeScheme()
        {
            AccountState &= ~AccountState.HasPayeScheme;
            
            return this;
        }
        
        public PortalClientTestsFixture ArrangeRecruitApiCallFails()
        {
            Vacancies = null;

            return this;
        }

        //todo: ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(int numberOfVacancies)?
        public PortalClientTestsFixture ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies()
        {
            Vacancies = Enumerable.Empty<Vacancy>();

            return this;
        }
        
        public PortalClientTestsFixture ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy()
        {
            Vacancies = Enumerable.Repeat(Vacancy, 1);

            return this;
        }

        public PortalClientTestsFixture ArrangeRecruitApiCallSucceedsAndReturnsTwoVacancies()
        {
            Vacancies = Enumerable.Repeat(Vacancy, 2);

            return this;
        }

        public async Task<Account> GetAccount()
        {
            // arrange
            MockDasRecruitService.Setup(s => s.GetVacancies(PublicHashedAccountId,2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Vacancies);

            OriginalVacancy = Vacancy.Clone();

            // act
            return await PortalClient.GetAccount(PublicHashedAccountId, AccountState);
        }

        public void AssertNullIsReturned(Account account)
        {
            account.Should().BeNull();
        }
        
        public void AssertVacancyCardinalityIsNotSet(Account account)
        {
            account.Should().NotBeNull();
            account.VacancyCardinality.Should().BeNull();
        }

        public void AssertVacancyCardinalityIsSet(Account account, Cardinality expectedCardinality)
        {
            account.Should().NotBeNull();
            account.VacancyCardinality.Should().Be(expectedCardinality);
        }
        
        public void AssertSingleVacancyIsNotSet(Account account)
        {
            account.Should().NotBeNull();
            account.SingleVacancy.Should().BeNull();
        }

        public void AssertSingleVacancyIsSetCorrectly(Account account)
        {
            account.Should().NotBeNull();
            account.SingleVacancy.Should().NotBeNull();
            account.SingleVacancy.Should().BeEquivalentTo(OriginalVacancy);
        }
    }
}