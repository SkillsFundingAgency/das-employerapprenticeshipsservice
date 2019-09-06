using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.StatisticsControllerTests
{
    [TestFixture]
    public class WhenIGetTheStatistics : GivenEmployerAccountsApi.GivenEmployerAccountsApi
    {
        private Statistics _expectedStatisticsViewModel;

        [SetUp]
        public async Task SetUp()
        {
            var accountStatisticsDataHelper = new AccountStatisticsDataHelper();

            _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            if (AnyAccountStatisticsAreZero(_expectedStatisticsViewModel))
            {
                await accountStatisticsDataHelper.CreateAccountStatistics();
                _expectedStatisticsViewModel = await accountStatisticsDataHelper.GetStatistics();
            }

            WhenControllerActionIsCalled(@"https://localhost:44330/api/statistics");
        }

        private static bool AnyAccountStatisticsAreZero(Statistics accountStatistics)
        {
            return accountStatistics.TotalAccounts == 0
                   || accountStatistics.TotalAgreements == 0
                   || accountStatistics.TotalLegalEntities == 0
                   || accountStatistics.TotalPayeSchemes == 0;
        }

        [Test]
        public void ThenTheStatusShouldBeOk()
        {
            Response.StatusCode
                .Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void ThenTotalAccountsIsCorrect()
        {
            Response.Content.ReadAsAsync<Statistics>().Result.TotalAccounts
                .Should().Be(_expectedStatisticsViewModel.TotalAccounts);
        }

        [Test]
        public void ThenTotalAgreementsIsCorrect()
        {
            Response.Content.ReadAsAsync<Statistics>().Result.TotalAgreements
                .Should().Be(_expectedStatisticsViewModel.TotalAgreements);
        }

        [Test]
        public void ThenTotalLegalEntitiesIsCorrect()
        {
            Response.Content.ReadAsAsync<Statistics>().Result.TotalLegalEntities
                .Should().Be(_expectedStatisticsViewModel.TotalLegalEntities);
        }

        [Test]
        public void ThenTotalPayeSchemesIsCorrect()
        {
            Response.Content.ReadAsAsync<Statistics>().Result.TotalPayeSchemes
                .Should().Be(_expectedStatisticsViewModel.TotalPayeSchemes);
        }
    }
}
