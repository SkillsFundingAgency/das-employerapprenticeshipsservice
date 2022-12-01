using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Transfers;

namespace SFA.DAS.EmployerFinance.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingGetCountsRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            long accountId = 123;

            var actual = new GetCountsRequest(accountId);

            actual.GetUrl.Should().Be($"transfers/{accountId}/counts");
        }
    }
}