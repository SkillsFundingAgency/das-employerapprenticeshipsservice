using System;
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
        //todo: tests for <0 maxNumberOfVacancies, 1, 2, max int

        #region Argument Guards
        
        [Test]
        public Task GetAccount_WhenGetAccountParameterIsNull_ThenArgumentNullExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeGetAccountParameterIsNull(),
                f => f.GetAccount(),
                (f, r) => r.Should().Throw<ArgumentNullException>());
        }

        [Test]
        public Task GetAccount_WhenHashedAccountIdIsNull_ThenArgumentExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeHashedAccountIdIsNull(),
                f => f.GetAccount(),
                (f, r) => r.Should().Throw<ArgumentException>());
        }

        [Test]
        public Task GetAccount_WhenMaxNumberOfVacanciesIsNegative_ThenArgumentOutOfRangeExceptionIsThrown()
        {
            return TestExceptionAsync(f => f.ArrangeMaxNumberOfVacanciesIsNegative(),
                f => f.GetAccount(),
                (f, r) => r.Should().Throw<ArgumentOutOfRangeException>());
        }

        #endregion Argument Guards

        #region Account Does Not Exist
        
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

        #endregion Account Does Not Exist

        #region Account Does Exist
        
        #region Max Vacancies Is 0
        
        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs0_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs0_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }
        
        #endregion Max Vacancies Is 0

        #region Max Vacancies Is > 0
        
        #region Recruit Api Call Fails
        
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

        #endregion Recruit Api Call Fails

        #endregion Max Vacancies Is > 0

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNoVacancies(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancy_ThenOneCorrectlyPopulatedVacancyIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertOneCorrectlyPopulatedVacancyIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancy_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsOneVacancy(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }

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

        #endregion Account Does Exist
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
        GetAccountParameters GetAccountParameters { get; set; } = new GetAccountParameters
        {
            HashedAccountId = HashedAccountId
        };

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

        public PortalClientTestsFixture ArrangeGetAccountParameterIsNull()
        {
            GetAccountParameters = null;

            return this;
        }

        public PortalClientTestsFixture ArrangeHashedAccountIdIsNull()
        {
            GetAccountParameters.HashedAccountId = null;

            return this;
        }

        public PortalClientTestsFixture ArrangeMaxNumberOfVacanciesIsNegative()
        {
            GetAccountParameters.MaxNumberOfVacancies = -1;

            return this;
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
            GetAccountParameters.MaxNumberOfVacancies = maxNumberOfVacancies;

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
            return await PortalClient.GetAccount(GetAccountParameters);
        }

        public void AssertNullIsReturned(Account account)
        {
            account.Should().BeNull();
        }

        public void AssertNoVacanciesAreReturned(Account account)
        {
            account.Vacancies.Should().BeEmpty();
        }

        public void AssertOneCorrectlyPopulatedVacancyIsReturned(Account account)
        {
            account.Should().NotBeNull();
            account.Vacancies.Should().NotBeNull();
            account.Vacancies.Should().BeEquivalentTo(OriginalVacancy);
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