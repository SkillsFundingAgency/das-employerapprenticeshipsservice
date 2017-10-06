using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAccountUsersByInternalId : ApiClientTestBase
    {
        private TeamMemberViewModel _teamMember;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/internal/{NumericalAccountId}/users";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _teamMember = new TeamMemberViewModel
            {
                Name = "Name",
                UserRef = "2163",
                Email = "test@test.com",
                Role = "Viewer"
            };

            var members = new List<TeamMemberViewModel> { _teamMember };

            HttpClient.Setup(c => c.GetAsync(absoluteUri))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(members)));
        }

        [Test]
        public async Task ThenTheCorrectEndpointIsCalled()
        {
            //Act
            var actual = await ApiClient.GetAccountUsers(NumericalAccountId);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Any());
        }
    }
}
