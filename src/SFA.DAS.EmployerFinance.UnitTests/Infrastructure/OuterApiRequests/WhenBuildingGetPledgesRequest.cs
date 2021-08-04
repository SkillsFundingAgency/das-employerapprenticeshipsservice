using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;

namespace SFA.DAS.EmployerFinance.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingGetPledgesRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            var accountId = 123;

            var actual = new GetPledgesRequest(accountId);

            actual.GetUrl.Should().Be($"Pledges?accountId={accountId}");
        }
    }
}