using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetActivitiesTests
{
    public class WhenIGetActivities
    {
        private const string HashedAccountId = "ABC123";
        private const string ExternalUserId = "ABCDEF";
        private const long AccountId = 111111;
        private const long UserId = 123456;

        private GetActivitiesQuery _query;
        private GetActivitiesQueryHandler _handler;
        private GetActivitiesResponse _response;
        private CurrentUser _currentUser;
        private Mock<IActivitiesClient> _activitiesClient;
        private Mock<ILog> _logger;
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly MembershipView _membershipView = new MembershipView { AccountId = AccountId, UserId = UserId };
        private readonly ActivitiesResult _activitiesResult = new ActivitiesResult();

        [SetUp]
        public void Arrange()
        {
            _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
            _activitiesClient = new Mock<IActivitiesClient>();
            _logger = new Mock<ILog>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _activitiesClient.Setup(c => c.GetActivities(It.IsAny<ActivitiesQuery>())).ReturnsAsync(_activitiesResult);

            _query = new GetActivitiesQuery
            {
                HashedAccountId = HashedAccountId,
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

            _handler = new GetActivitiesQueryHandler(_currentUser, _activitiesClient.Object, _logger.Object, _membershipRepository.Object);
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(HashedAccountId, ExternalUserId), Times.Once);
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

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_query));
        }

        [Test]
        public async Task ThenShouldReturnGetActivitiesResponseIfClientThrowsAnException()
        {
            _activitiesClient.Setup(c => c.GetActivities(It.IsAny<ActivitiesQuery>())).Throws<Exception>();

            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.Result, Is.Not.Null);
            Assert.That(_response.Result.Activities, Is.Empty);
            Assert.That(_response.Result.Total, Is.Zero);
        }
    }
}