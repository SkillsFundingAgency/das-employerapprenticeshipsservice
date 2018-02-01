using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLatestActivitiesTests
{
    public class WhenIGetLatestActivities
    {
        private const string HashedAccountId = "ABC123";
        private const string ExternalUserId = "ABCDEF";
        private const long AccountId = 111111;
        private const long UserId = 123456;

        private GetLatestActivitiesQuery _query;
        private GetLatestActivitiesQueryHandler _handler;
        private GetLatestActivitiesResponse _response;
        private CurrentUser _currentUser;
        private Mock<IActivitiesClient> _activitiesClient;
        private Mock<IMembershipRepository> _membershipRepository;
        private readonly MembershipView _membershipView = new MembershipView { AccountId = AccountId, UserId = UserId };
        private readonly AggregatedActivitiesResult _latestActivitiesResult = new AggregatedActivitiesResult();

        [SetUp]
        public void Arrange()
        {
            _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
            _activitiesClient = new Mock<IActivitiesClient>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _activitiesClient.Setup(c => c.GetLatestActivities(It.IsAny<long>())).ReturnsAsync(_latestActivitiesResult);

            _query = new GetLatestActivitiesQuery
            {
                HashedAccountId = HashedAccountId
            };

            _handler = new GetLatestActivitiesQueryHandler(_currentUser, _activitiesClient.Object, _membershipRepository.Object);
        }

        [Test]
        public void ThenShouldGetUser()
        {
            _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(HashedAccountId, ExternalUserId), Times.Once);
        }

        [Test]
        public void ThenShouldGetLatestActivities()
        {
            _handler.Handle(_query);

            _activitiesClient.Verify(c => c.GetLatestActivities(AccountId), Times.Once);
        }

        [Test]
        public void ThenShouldReturnGetLatestActivitiesResponse()
        {
            _response = _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.Result, Is.Not.Null);
            Assert.That(_response.Result, Is.EqualTo(_latestActivitiesResult));
        }

        [Test]
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.Throws<UnauthorizedAccessException>(() => _handler.Handle(_query));
        }
    }
}
