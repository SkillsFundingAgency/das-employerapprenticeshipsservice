using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetActivities;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetActivitiesTests
{
    [TestFixture]
    public class WhenIGetActivities
    {
        private const long AccountId = 111111;

        private GetActivitiesQuery _query;
        private GetActivitiesQueryHandler _handler;
        private GetActivitiesResponse _response;
        private Mock<IActivitiesClient> _activitiesClient;
        private readonly ActivitiesResult _activitiesResult = new ActivitiesResult();

        [SetUp]
        public void Arrange()
        {
            _activitiesClient = new Mock<IActivitiesClient>();
            
            _activitiesClient.Setup(c => c.GetActivities(It.IsAny<ActivitiesQuery>())).ReturnsAsync(_activitiesResult);

            _query = new GetActivitiesQuery
            {
                AccountId = AccountId,
                Take = 100,
                From = DateTime.UtcNow.AddDays(-1),
                To = DateTime.UtcNow,
                Term = "Foo Bar",
                Category = ActivityTypeCategory.Unknown,
                Data = new Dictionary<string, string>
                {
                    ["Foo"] = "Bar"
                }
            };

            _handler = new GetActivitiesQueryHandler(_activitiesClient.Object);
        }

        [Test]
        public async Task ThenShouldGetActivities()
        {
            await _handler.Handle(_query);
            
            _activitiesClient.Verify(c => c.GetActivities(It.Is<ActivitiesQuery>(q => 
                q.AccountId == AccountId &&
                q.Take == _query.Take &&
                q.From == _query.From &&
                q.To == _query.To &&
                q.Term == _query.Term &&
                q.Category == _query.Category &&
                q.Data == _query.Data
            )), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetActivitiesResponse()
        {
            _response = await _handler.Handle(_query);
            
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.Result, Is.Not.Null);
            Assert.That(_response.Result, Is.EqualTo(_activitiesResult));
        }
    }
}