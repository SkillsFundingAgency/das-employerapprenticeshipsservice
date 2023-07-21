using System.Diagnostics.CodeAnalysis;
using System.Net;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.ModelBuilders;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.EmployerAccountControllerTests;

[ExcludeFromCodeCoverage]
[TestFixture]
public class WhenGetAccountWithKnownIds : GivenEmployerAccountsApi
{
    [SetUp]
    public async Task SetUp()
    {
        EmployerAccountOutput? employerAccount = null;

        await InitialiseEmployerAccountData(async builder =>
        {
            var data = new TestModelBuilder()
                .WithNewUser()
                .WithNewAccount()
                .WithNewLegalEntity();

            await builder.SetupDataAsync(data);

            employerAccount = data.CurrentAccount.AccountOutput;
        });

        WhenControllerActionIsCalled($"/api/accounts/{employerAccount?.HashedAccountId}");
    }

    [Test]
    public void ThenTheStatusShouldBeFound_ByHashedAccountId()
    {
        Response.ExpectStatusCodes(HttpStatusCode.OK);
        Assert.IsNotNull(Response.GetContent<AccountDetail>());
    }
}