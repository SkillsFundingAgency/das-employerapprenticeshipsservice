using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;

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
            var financialStatistics = await financeStatisticsDataHelper.GetStatistics();
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

    public class FinanceStatisticsDataHelper
    {
        private readonly string _databaseConnectionString;

        public FinanceStatisticsDataHelper(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_databaseConnectionString))
            {
                
                return await connection.QuerySingleAsync<StatisticsViewModel>(Sql);
            }
        }

        private const string Sql = @"
select count(0) as TotalPayments
from employer_financial.Payment;";
    }

    internal class AccountStatisticsDataHelper
    {
        private readonly string _databaseConnectionString;
        
        public AccountStatisticsDataHelper(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }

        public async Task<StatisticsViewModel> GetStatistics()
        {
            using (var connection = new SqlConnection(_databaseConnectionString))
            {
                
                return await connection.QuerySingleAsync<StatisticsViewModel>(Sql);
            }
        }

        private const string Sql = @"
select (
  select count(0)
  from employer_account.Account
) as TotalAccounts, (
  select count(0)
  from employer_account.LegalEntity
) as TotalLegalEntities, (
  select count(0)
  from employer_account.Paye
) as TotalPayeSchemes, (
  select count(0)
  from employer_account.EmployerAgreement a
  where a.StatusId = 2 -- signed
) as TotalAgreements;";
    }
}
