using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using TeamMember = SFA.DAS.EmployerAccounts.Api.Types.TeamMember;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public class WhenIGetAnAccountsUsers : EmployerAccountsControllerTests
    {
        [Test]
        public async Task ThenTheAccountUsersAreReturned()
        {
            var hashedAccountId = "ABC123";

            var accountsUserResponse = new GetAccountTeamMembersResponse
            {
                TeamMembers = new List<Models.AccountTeam.TeamMember>
                {
                    new Models.AccountTeam.TeamMember
                    { 
                        HashedAccountId = hashedAccountId,
                        Email = "test@test'com",
                        Name = "Test"
                    }
                }
            };

            Mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTeamMembersQuery>()))
                .ReturnsAsync(accountsUserResponse);
          
            var response = await Controller.GetAccountUsers(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<TeamMember>>>(response);
        }
    }
}
