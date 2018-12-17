using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions.Common;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    public class WhenGettingAccountUsers : ApiClientTestBase
    {
        private TeamMemberViewModel _teamMember;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/users";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;
            var fixture = new Fixture();

            _teamMember = fixture.Create<TeamMemberViewModel>();

            var members = new List<TeamMemberViewModel> { _teamMember };

            HttpClient
                .Setup(c => c.GetAsync(absoluteUri))
                .Returns(Task.FromResult(JsonConvert.SerializeObject(members)));
        }

        [Test]
        public async Task ThenThePayeSchemeIsReturned()
        {
            // Act
            var response = await ApiClient.GetAccountUsers(TextualAccountId);
            var viewModel = response?.FirstOrDefault();

            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(viewModel);
            viewModel.IsSameOrEqualTo(_teamMember);
        }
    }
}
