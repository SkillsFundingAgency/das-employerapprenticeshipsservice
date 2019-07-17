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
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Encoding;
using SFA.DAS.Testing;
using StructureMap;

//todo: update tests for new client
namespace SFA.DAS.EAS.Portal.UnitTests.Client
{
    /// <summary>
    /// These tests don't cover every permutation, only a (sensible) subset
    /// </summary>
    [TestFixture, Parallelizable]
    public class PortalClientTests : FluentTest<PortalClientTestsFixture>
    {
        //todo: tests for null/empty hashedAccountId
        //todo: tests for <0 maxNumberOfVacancies, 1, 2, max int
        
        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs0_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallFails_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallFails_ThenVacanciesRetrievedIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsNotSet(r));
        }

//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallFails_ThenSingleVacancyIsNotSet()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
//                    .ArrangeHasPayeScheme().ArrangeRecruitApiCallFails(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertSingleVacancyIsNotSet(r));
//        }
//
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenVacancyCardinalityIsSetToNone()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
//                    .ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.None));
//        }
//
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenSingleVacancyIsNotSet()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
//                    .ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertSingleVacancyIsNotSet(r));
//        }
//        
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenVacancyCardinalityIsSetToOne()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.One));
//        }
//
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenSingleVacancyIsSetCorrectly()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertSingleVacancyIsSetCorrectly(r));
//        }
//
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenVacancyCardinalityIsSetToMany()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsTwoVacancies(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertVacancyCardinalityIsSet(r, Cardinality.Many));
//        }
//
//        [Test]
//        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndHasPayeSchemeAndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenSingleVacancyIsNotSet()
//        {
//            return TestAsync(f => f.ArrangeAccountExists().ArrangeHasPayeScheme().ArrangeRecruitApiCallSucceedsAndReturnsTwoVacancies(),
//                f => f.GetAccount(),
//                (f, r) => f.AssertSingleVacancyIsNotSet(r));
//        }
    }

    public class PortalClientTestsFixture
    {
        PortalClient PortalClient { get; set; }
        Mock<IContainer> MockContainer { get; set; } = new Mock<IContainer>();
        private Mock<IEncodingService> MockEncodingService { get; set; } = new Mock<IEncodingService>();
        Mock<IAccountsReadOnlyRepository> MockAccountsReadOnlyRepository { get; set; } = new Mock<IAccountsReadOnlyRepository>();
        Mock<IDasRecruitService> MockDasRecruitService { get; set; } = new Mock<IDasRecruitService>();
        Account Account { get; set; }
        Vacancy Vacancy { get; set; }
        Vacancy OriginalVacancy { get; set; }
        IEnumerable<Vacancy> Vacancies { get; set; }
        const long AccountId = 999L;
        const string HashedAccountId = "HASH12";
        int MaxNumberOfVacancies;

        public PortalClientTestsFixture()
        {
            Vacancy = new Fixture().Create<Vacancy>();
            
            Account = JsonConvert.DeserializeObject<Account>($"{{\"Id\": {AccountId} }}");

            MockContainer.Setup(c => c.GetInstance<IAccountsReadOnlyRepository>())
                .Returns(MockAccountsReadOnlyRepository.Object);
            MockContainer.Setup(c => c.GetInstance<IDasRecruitService>())
                .Returns(MockDasRecruitService.Object);

            MockEncodingService.Setup(s => s.Decode(HashedAccountId, EncodingType.AccountId))
                .Returns(AccountId);
            
            PortalClient = new PortalClient(MockContainer.Object, MockEncodingService.Object);
        }

        public PortalClientTestsFixture ArrangeAccountExists()
        {
            MockAccountsReadOnlyRepository.Setup(q => q.Get(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Account);
            
            return this;
        }

        public PortalClientTestsFixture ArrangeAccountDoesNotExist()
        {
            MockAccountsReadOnlyRepository.Setup(q => q.Get(AccountId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(Account));
            
            return this;
        }

        public PortalClientTestsFixture ArrangeMaxNumberOfVacancies(int maxNumberOfVacancies)
        {
            MaxNumberOfVacancies = maxNumberOfVacancies;

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
            MockDasRecruitService.Setup(s => s.GetVacancies(HashedAccountId,2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Vacancies);

            OriginalVacancy = Vacancy.Clone();

            // act
            return await PortalClient.GetAccount(HashedAccountId, MaxNumberOfVacancies);
        }

        public void AssertNullIsReturned(Account account)
        {
            account.Should().BeNull();
        }

        public void AssertNoVacanciesAreReturned(Account account)
        {
            account.Vacancies.Should().BeEmpty();
        }

        public void AssertVacanciesRetrievedIsSet(Account account)
        {
            account.Should().NotBeNull();
            account.VacanciesRetrieved.Should().BeTrue();
        }

        public void AssertVacanciesRetrievedIsNotSet(Account account)
        {
            account.Should().NotBeNull();
            account.VacanciesRetrieved.Should().BeFalse();
        }

//        public void AssertVacancyCardinalityIsNotSet(Account account)
//        {
//            account.Should().NotBeNull();
//            account.VacancyCardinality.Should().BeNull();
//        }
//
//        public void AssertVacancyCardinalityIsSet(Account account, Cardinality expectedCardinality)
//        {
//            account.Should().NotBeNull();
//            account.VacancyCardinality.Should().Be(expectedCardinality);
//        }
//        
//        public void AssertSingleVacancyIsNotSet(Account account)
//        {
//            account.Should().NotBeNull();
//            account.SingleVacancy.Should().BeNull();
//        }
//
//        public void AssertSingleVacancyIsSetCorrectly(Account account)
//        {
//            account.Should().NotBeNull();
//            account.SingleVacancy.Should().NotBeNull();
//            account.SingleVacancy.Should().BeEquivalentTo(OriginalVacancy);
//        }
    }
}