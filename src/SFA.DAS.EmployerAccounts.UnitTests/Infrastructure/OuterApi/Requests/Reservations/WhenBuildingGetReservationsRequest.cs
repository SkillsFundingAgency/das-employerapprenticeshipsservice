using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Reservations;

namespace SFA.DAS.EmployerAccounts.UnitTests.Infrastructure.OuterApi.Requests.Reservations;

public class WhenBuildingGetReservationsRequest
{
    [Test]
    public void Then_The_Url_Is_Correctly_Constructed()
    {
        long accountId = 123;

        var actual = new GetReservationsRequest(accountId);

        actual.GetUrl.Should().Be($"reservation/{accountId}");
    }
}