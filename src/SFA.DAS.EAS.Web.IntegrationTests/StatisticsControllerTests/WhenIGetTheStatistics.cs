using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
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
using SFA.DAS.Provider.Events.Api.Types;

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
            var call = new CallRequirements("api/statistics")
                .AllowStatusCodes(HttpStatusCode.OK);
            _apiTester = new ApiIntegrationTester();
            var accountStatisticsDataHelper = new AccountStatisticsDataHelper(_apiTester.EmployerApprenticeshipsServiceConfiguration.DatabaseConnectionString);
            var financeStatisticsDataHelper = new FinanceStatisticsDataHelper(_apiTester.LevyDeclarationProviderConfiguration.DatabaseConnectionString);
            
            _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            if (_expectedStatisticsViewModel.TotalAccounts == 0
                || _expectedStatisticsViewModel.TotalAgreements == 0
                || _expectedStatisticsViewModel.TotalLegalEntities == 0
                || _expectedStatisticsViewModel.TotalPayeSchemes == 0)
            {
                using (var connection = new SqlConnection(_apiTester.EmployerApprenticeshipsServiceConfiguration.DatabaseConnectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    var accountDbContext = new EmployerAccountsDbContext(connection, transaction);
                    var lazyDb = new Lazy<EmployerAccountsDbContext>(() => accountDbContext);
                    var accountRepo = new AccountRepository(_apiTester.EmployerApprenticeshipsServiceConfiguration,
                        Mock.Of<ILog>(), lazyDb, Mock.Of<IAccountLegalEntityPublicHashingService>());
                    await accountRepo.CreateAccount(10003/**/, "234", "integration-tests", "address", DateTime.Today, "emp-ref", 
                        "access-token", "refresh-token", "company-status", "emp ref name", 2, 1, "sector");
                    transaction.Commit();
                }
                
                _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            }

            var financialStatistics = await financeStatisticsDataHelper.GetStatistics();
            if (financialStatistics.TotalPayments == 0)
            {
                var financeDbContext = new EmployerFinanceDbContext(_apiTester.LevyDeclarationProviderConfiguration.DatabaseConnectionString);
                var lazyDb = new Lazy<EmployerFinanceDbContext>(() => financeDbContext);
                var levyRepository = new DasLevyRepository(_apiTester.LevyDeclarationProviderConfiguration,
                    Mock.Of<ILog>(), lazyDb);
                await levyRepository.CreatePayments(new List<PaymentDetails>
                {
                    new PaymentDetails
                    {
                        ProviderName = "prov name",
                        StandardCode = 31,
                        ProgrammeType = 3,
                        CourseName = "course name",
                        ApprenticeName = "apprentice name",
                        ApprenticeNINumber = "nin",
                        CourseLevel = 3,
                        CourseStartDate = DateTime.Today,

                        Ukprn = 134234,
                        Uln   = 323423,
                        EmployerAccountId = 342,
                        ApprenticeshipId = 1,
                        DeliveryPeriodMonth = 5,
                        DeliveryPeriodYear = 2018,
                        CollectionPeriodId = "R05",
                        CollectionPeriodMonth = 4,
                        CollectionPeriodYear = 2018,
                        EvidenceSubmittedOn = DateTime.Today,
                        EmployerAccountVersion = "emp acct ver",
                        ApprenticeshipVersion = "appr ver",
                        FundingSource = FundingSource.Levy,
                        TransactionType = TransactionType.Learning,
                        Amount = 234,
                        PeriodEnd = "R06"
                    }
                });
                financeDbContext.Database.CurrentTransaction.Commit();

                financialStatistics = await financeStatisticsDataHelper.GetStatistics();
            }

            _expectedStatisticsViewModel.TotalPayments = financialStatistics.TotalPayments;

            _actualResponse = await _apiTester.InvokeGetAsync<StatisticsViewModel>(call);
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
