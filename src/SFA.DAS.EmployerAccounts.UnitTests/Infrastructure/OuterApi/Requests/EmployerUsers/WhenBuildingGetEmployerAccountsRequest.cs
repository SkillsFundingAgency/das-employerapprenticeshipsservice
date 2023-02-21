using System.Net;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.UserAccounts;

namespace SFA.DAS.EmployerAccounts.UnitTests.Infrastructure.OuterApi.Requests.EmployerUsers;

public class WhenBuildingGetEmployerAccountsRequest
{
    [Test, AutoData]
    public void Then_The_Url_Is_Correctly_Formatted_With_Encoded_Email(string email, string userId)
    {
        email = email + "'test @+£@$" + email;

        var actual = new GetUserAccountsRequest(email, userId);

        actual.GetUrl.Should().Be($"accountusers/{userId}/accounts?email={WebUtility.UrlEncode(email)}");
    }
}