using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
                using (var connection = new SqlConnection(_apiTester.EmployerApprenticeshipsServiceConfiguration.DatabaseConnectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    var accountDbContext = new EmployerAccountsDbContext(connection, transaction);
                    var lazyDb = new Lazy<EmployerAccountsDbContext>(() => accountDbContext);
                    var accountRepo = new AccountRepository(_apiTester.EmployerApprenticeshipsServiceConfiguration,
                        Mock.Of<ILog>(), lazyDb, Mock.Of<IAccountLegalEntityPublicHashingService>());
                    await accountRepo.CreateAccount(10003/*todo:get proper id*/, "234", "emp name", "address", DateTime.Today, $"ref-{DateTime.Now.Ticks}", 
                        "access-token", "refresh-token", "company-status", "emp ref name", 2, 1, "sector");
                    transaction.Commit();
                }
                
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
