using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetLatestActivitiesTests
{
    [TestFixture]
    public class WhenIGetLatestActivities
    {
        private const long AccountId = 111111;

        private GetLatestActivitiesQuery _query;
        private GetLatestActivitiesQueryHandler _handler;
        private GetLatestActivitiesResponse _response;
        private Mock<IActivitiesClient> _activitiesClient;
        private readonly AggregatedActivitiesResult _latestActivitiesResult = new AggregatedActivitiesResult();

        [SetUp]
        public void Arrange()
        {
            _activitiesClient = new Mock<IActivitiesClient>();

            _activitiesClient.Setup(c => c.GetLatestActivities(It.IsAny<long>())).ReturnsAsync(_latestActivitiesResult);

            _query = new GetLatestActivitiesQuery
            {
                AccountId = AccountId
            };

            _handler = new GetLatestActivitiesQueryHandler(_activitiesClient.Object);
        }

        [Test]
        public async Task ThenShouldGetLatestActivities()
        {
            await _handler.Handle(_query, CancellationToken.None);

            _activitiesClient.Verify(c => c.GetLatestActivities(AccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetLatestActivitiesResponse()
        {
            _response = await _handler.Handle(_query, CancellationToken.None);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.Result, Is.Not.Null);
            Assert.That(_response.Result, Is.EqualTo(_latestActivitiesResult));
        }
    }
}
