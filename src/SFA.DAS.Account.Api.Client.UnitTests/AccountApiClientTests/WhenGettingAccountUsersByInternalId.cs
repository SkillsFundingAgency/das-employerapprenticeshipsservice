using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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

            var fixture = new Fixture();

            _teamMember = fixture.Create<TeamMemberViewModel>();

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
