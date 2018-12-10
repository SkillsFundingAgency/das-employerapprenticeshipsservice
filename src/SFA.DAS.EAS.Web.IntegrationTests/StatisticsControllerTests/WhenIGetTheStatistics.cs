using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.Hashing;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.StatisticsControllerTests
{
    [TestFixture]
    public class WhenIGetTheStatistics
    {
        private ApiIntegrationTester _apiTester;
        private CallResponse<StatisticsViewModel> _actualResponse;
        private StatisticsViewModel _expectedStatisticsViewModel;

        [SetUp]
        public async Task Setup()
        {
            var fixture = new Fixture();
            _apiTester = new ApiIntegrationTester();
            var accountStatisticsDataHelper = new AccountStatisticsDataHelper(_apiTester.EmployerApprenticeshipsServiceConfiguration.DatabaseConnectionString);
            var financeStatisticsDataHelper = new FinanceStatisticsDataHelper(_apiTester.LevyDeclarationProviderConfiguration.DatabaseConnectionString);
            
            _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            if (AnyAccountStatisticsAreZero(_expectedStatisticsViewModel))
            {
                var accountDbContext = new EmployerAccountsDbContext(_apiTester.EmployerApprenticeshipsServiceConfiguration.DatabaseConnectionString);
                accountDbContext.Database.BeginTransaction();
                var lazyDb = new Lazy<EmployerAccountsDbContext>(() => accountDbContext);
                var accountRepo = new AccountRepository(_apiTester.EmployerApprenticeshipsServiceConfiguration,
                    Mock.Of<ILog>(), lazyDb, Mock.Of<IAccountLegalEntityPublicHashingService>());
                var userRepo = new UserRepository(_apiTester.EmployerApprenticeshipsServiceConfiguration, Mock.Of<ILog>(), lazyDb);
                var newUser = fixture
                    .Build<User>()
                    .Without(user => user.Id)
                    .Without(user => user.UserRef)
                    .Create();
                await userRepo.Create(newUser);
                var x = await userRepo.GetUserByRef(newUser.UserRef);
                await accountRepo.CreateAccount(
                    x.Id, 
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    DateTime.Today, 
                    $"ref-{DateTime.Now.Ticks.ToString().Substring(4,10)}", 
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    2, 
                    1, 
                    fixture.Create<string>());
                accountDbContext.Database.CurrentTransaction.Commit();
                
                _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            }

            var financialStatistics = await financeStatisticsDataHelper.GetStatistics();
            if (AnyFinanceStatisticsAreZero(financialStatistics))
            {
                var financeDbContext = new EmployerFinanceDbContext(_apiTester.LevyDeclarationProviderConfiguration.DatabaseConnectionString);
                financeDbContext.Database.BeginTransaction();
                var lazyDb = new Lazy<EmployerFinanceDbContext>(() => financeDbContext);
                var levyRepository = new DasLevyRepository(_apiTester.LevyDeclarationProviderConfiguration, Mock.Of<ILog>(), lazyDb);
                await levyRepository.CreatePayments(new List<PaymentDetails>
                {
                    fixture
                        .Build<PaymentDetails>()
                        .With(details => details.CollectionPeriodId, "R05")
                        // could put sanitised collection period and delivery period values in for mth and year
                        .With(details => details.PeriodEnd, "R12")
                        .With(details => details.EmployerAccountVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4,10)}")
                        .With(details => details.ApprenticeshipVersion, $"ver-{DateTime.Now.Ticks.ToString().Substring(4,10)}")
                        .Without(details => details.FrameworkCode)
                        .Without(details => details.PathwayCode)
                        .Without(details => details.PathwayName)
                        .Create()
                });
                financeDbContext.Database.CurrentTransaction.Commit();

                financialStatistics = await financeStatisticsDataHelper.GetStatistics();
            }

            _expectedStatisticsViewModel.TotalPayments = financialStatistics.TotalPayments;

            _actualResponse = await _apiTester.InvokeGetAsync<StatisticsViewModel>(new CallRequirements("api/statistics"));
        }

        private static bool AnyAccountStatisticsAreZero(StatisticsViewModel accountStatistics)
        {
            return accountStatistics.TotalAccounts == 0
                   || accountStatistics.TotalAgreements == 0
                   || accountStatistics.TotalLegalEntities == 0
                   || accountStatistics.TotalPayeSchemes == 0;
        }

        private static bool AnyFinanceStatisticsAreZero(StatisticsViewModel financialStatistics)
        {
            return financialStatistics.TotalPayments == 0;
        }

        [Test]
        public void ThenTheStatusShouldBeOk()
        {
            _actualResponse.Response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ThenTotalAccountsIsCorrect()
        {
            _actualResponse.Data.TotalAccounts
                .Should().Be(_expectedStatisticsViewModel.TotalAccounts);
        }

        [Test]
        public void ThenTotalAgreementsIsCorrect()
        {
            _actualResponse.Data.TotalAgreements
                .Should().Be(_expectedStatisticsViewModel.TotalAgreements);
        }

        [Test]
        public void ThenTotalLegalEntitiesIsCorrect()
        {
            _actualResponse.Data.TotalLegalEntities
                .Should().Be(_expectedStatisticsViewModel.TotalLegalEntities);
        }

        [Test]
        public void ThenTotalPayeSchemesIsCorrect()
        {
            _actualResponse.Data.TotalPayeSchemes
                .Should().Be(_expectedStatisticsViewModel.TotalPayeSchemes);
        }

        [Test]
        public void ThenTotalPaymentsIsCorrect()
        {
            _actualResponse.Data.TotalPayments
                .Should().Be(_expectedStatisticsViewModel.TotalPayments);
        }
    }
}
