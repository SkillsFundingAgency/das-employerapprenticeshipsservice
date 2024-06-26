using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;
using SFA.DAS.EAS.Domain.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.ApplicationServices.Services;
using SFA.DAS.EAS.Support.Web.Controllers;
using SFA.DAS.EAS.Support.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EAS.Support.Web.Tests.Controllers.UserRoles;

public class WhenTestingIndexGet
{
    [Test, MoqAutoData]
    public async Task ItShouldReturnNotFoundWhenFindTeamMembersFails(
        Mock<IAccountHandler> accountHandler,
        Mock<IEmployerAccountsApiService> accountsApiService,
        string hashedAccountId
    )
    {
        var sut = new UserRolesController(accountHandler.Object, Mock.Of<ILogger<UserRolesController>>(), accountsApiService.Object);
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        accountHandler.Setup(x => x.FindTeamMembers(hashedAccountId)).ReturnsAsync(new AccountReponse
        {
            StatusCode = SearchResponseCodes.NoSearchResultsFound
        });

        var actual = await sut.Index(hashedAccountId, string.Empty);

        actual.Should().BeOfType<NotFoundResult>();
    }

    [Test, MoqAutoData]
    public async Task ItShouldReturnViewAndModel(
        Mock<IAccountHandler> accountHandler,
        Mock<IEmployerAccountsApiService> accountsApiService,
        ICollection<TeamMemberViewModel> teamMembers,
        string hashedAccountId,
        string userRef,
        Role role,
        string teamMemberName
    )
    {
        var sut = new UserRolesController(accountHandler.Object, Mock.Of<ILogger<UserRolesController>>(), accountsApiService.Object);
        sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        teamMembers.First().UserRef = userRef;
        teamMembers.First().Role = role.ToString();
        teamMembers.First().Name = teamMemberName;

        accountHandler.Setup(x => x.FindTeamMembers(hashedAccountId)).ReturnsAsync(new AccountReponse
        {
            StatusCode = SearchResponseCodes.Success,
            Account = new Core.Models.Account
            {
                HashedAccountId = hashedAccountId,
                TeamMembers = teamMembers
            }
        });

        var actual = await sut.Index(hashedAccountId, userRef);

        using (new AssertionScope())
        {
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            var model = ((ViewResult)actual).Model as ChangeRoleViewModel;

            model?.HashedAccountId.Should().Be(hashedAccountId);
            model?.UserRef.Should().Be(userRef);
            model?.Role.Should().Be(role);
            model?.Name.Should().Be(teamMemberName);
            model?.ResponseUrl.Should().Be($"/resource/role/change/{hashedAccountId}/{userRef}");
        }
    }
}