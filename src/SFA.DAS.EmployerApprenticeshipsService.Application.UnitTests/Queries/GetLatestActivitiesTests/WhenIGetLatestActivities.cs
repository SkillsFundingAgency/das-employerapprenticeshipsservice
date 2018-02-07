using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLatestActivitiesTests
{
    [TestFixture]
    public class WhenIGetLatestActivities
    {
        private const string AccountHashedId = "ABC123";
        private const long AccountId = 111111;

        private GetLatestActivitiesQuery _query;
        private GetLatestActivitiesQueryHandler _handler;
        private GetLatestActivitiesResponse _response;
        private Mock<IActivitiesClient> _activitiesClient;
        private Mock<IHashingService> _hashingService;
        private readonly AggregatedActivitiesResult _latestActivitiesResult = new AggregatedActivitiesResult();

        [SetUp]
        public void Arrange()
        {
            _activitiesClient = new Mock<IActivitiesClient>();
            _hashingService = new Mock<IHashingService>();

            _activitiesClient.Setup(c => c.GetLatestActivities(It.IsAny<long>())).ReturnsAsync(_latestActivitiesResult);
            _hashingService.Setup(h => h.DecodeValue(AccountHashedId)).Returns(AccountId);

            _query = new GetLatestActivitiesQuery
            {
                AccountHashedId = AccountHashedId
            };

            _handler = new GetLatestActivitiesQueryHandler(_activitiesClient.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenShouldGetLatestActivities()
        {
            await _handler.Handle(_query);

            _activitiesClient.Verify(c => c.GetLatestActivities(AccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetLatestActivitiesResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.Result, Is.Not.Null);
            Assert.That(_response.Result, Is.EqualTo(_latestActivitiesResult));
        }
    }
}
