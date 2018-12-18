using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerFinance.Api.IntegrationTests.TestUtils.DataAccess.DataHelpers;
using SFA.DAS.EmployerFinance.Models;

namespace SFA.DAS.EmployerFinance.Api.IntegrationTests.Service.StatisticsControllerTests
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
            _apiTester = new ApiIntegrationTester();
            var financeStatisticsDataHelper = new FinanceStatisticsDataHelper();

            _expectedStatisticsViewModel = await financeStatisticsDataHelper.GetStatistics();

            if (AnyFinanceStatisticsAreZero(_expectedStatisticsViewModel))
            {
                await financeStatisticsDataHelper.CreateFinanceStatistics();
                _expectedStatisticsViewModel = await financeStatisticsDataHelper.GetStatistics();
            }

            _actualResponse = await _apiTester.InvokeGetAsync<StatisticsViewModel>(new CallRequirements("api/statistics"));
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
