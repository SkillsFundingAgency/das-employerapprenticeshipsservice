using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.DataHelpers;

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
}
