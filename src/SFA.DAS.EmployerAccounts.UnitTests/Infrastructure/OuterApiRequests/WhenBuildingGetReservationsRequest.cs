using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApiRequests.Reservations;

namespace SFA.DAS.EmployerAccounts.UnitTests.Infrastructure.OuterApiRequests
{
    public class WhenBuildingGetReservationsRequest
    {
        [Test]
        public void Then_The_Url_Is_Correctly_Constructed()
        {
            long accountId = 123;

            var actual = new GetReservationsRequest(accountId);

            actual.GetUrl.Should().Be($"reservation/1");
        }
    }
}
