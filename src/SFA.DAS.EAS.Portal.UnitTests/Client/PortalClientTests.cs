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
using SFA.DAS.EAS.Portal.Client.Services.Recruit;
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

        #region Max Vacancies Is 0

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs0_ThenNullIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertNullIsReturned(r));
        }
        
        #endregion Max Vacancies Is 0
        
        #region Max Vacancies Is > 0
        
        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallFails_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallFails_ThenVacanciesRetrievedIsNotSet()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallFails(),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsNotSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }
        
        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenOneCorrectlyPopulatedVacancyIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(1),
                f => f.GetAccount(),
                (f, r) => f.AssertOneCorrectlyPopulatedVacancyIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountDoesNotExistAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancies_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountDoesNotExist().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(1),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }

        #endregion Max Vacancies Is > 0
        
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

        #region Recruit Api Call Succeeds

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenNoVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertNoVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsNoVacancies_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(0),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancy_ThenOneCorrectlyPopulatedVacancyIsReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(1),
                f => f.GetAccount(),
                (f, r) => f.AssertOneCorrectlyPopulatedVacancyIsReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsOneVacancy_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(1),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenTwoCorrectlyPopulatedVacanciesAreReturned()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(2),
                f => f.GetAccount(),
                (f, r) => f.AssertTwoCorrectlyPopulatedVacanciesAreReturned(r));
        }

        [Test]
        public Task GetAccount_WhenAccountExistsAndMaxVacanciesIs2AndRecruitApiCallSucceedsAndReturnsTwoVacancies_ThenVacanciesRetrievedIsSet()
        {
            return TestAsync(f => f.ArrangeAccountExists().ArrangeMaxNumberOfVacancies(2)
                    .ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(2),
                f => f.GetAccount(),
                (f, r) => f.AssertVacanciesRetrievedIsSet(r));
        }
        
        #endregion Recruit Api Call Succeeds
        
        #endregion Max Vacancies Is > 0

        #endregion Account Does Exist
    }

    public class PortalClientTestsFixture
    {
        PortalClient PortalClient { get; set; }
        Mock<IContainer> MockContainer { get; set; } = new Mock<IContainer>();
        private Mock<IEncodingService> MockEncodingService { get; set; } = new Mock<IEncodingService>();
        Mock<IAccountsReadOnlyRepository> MockAccountsReadOnlyRepository { get; set; } = new Mock<IAccountsReadOnlyRepository>();
        Mock<IRecruitService> MockDasRecruitService { get; set; } = new Mock<IRecruitService>();
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
            MockContainer.Setup(c => c.GetInstance<IRecruitService>())
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

        public PortalClientTestsFixture ArrangeRecruitApiCallSucceedsAndReturnsNumberOfVacancies(int numVacancies)
        {
            Vacancies = Enumerable.Repeat(Vacancy, numVacancies);

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

        public void AssertTwoCorrectlyPopulatedVacanciesAreReturned(Account account)
        {
            account.Should().NotBeNull();
            account.Vacancies.Should().NotBeNull();
            account.Vacancies.Should().BeEquivalentTo(Enumerable.Repeat(Vacancy, 2));
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
    }
}