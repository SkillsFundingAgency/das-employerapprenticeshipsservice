using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Http;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Data
{
    [TestFixture]
    public class IdamsTests
    {

        private IdamsEmailServiceWrapper _sut;
        private Mock<IHttpClientWrapper> _mockHttpClientWrapper;   

        [SetUp]
        public void SetUp()
        {
            var config = new EmployerApprenticeshipsServiceConfiguration
            {
                CommitmentNotification = new CommitmentNotificationConfiguration
                {
                    IdamsListUsersUrl =
                        "https://url.to/users/ukprn={0}"
                }
            };
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
            _sut = new IdamsEmailServiceWrapper(Mock.Of<ILogger>(), config, _mockHttpClientWrapper.Object, new NoopExecutionPolicy());
        }

        [Test]
        public async Task ShouldReturnEmpltyListIfResponseIsEmpty()
        {
            var res = await _sut.GetEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(0);
        }

        [Test]
        public async Task ShouldReturnEmpltyListIfResponseNotInCorrctFormat()
        {
            
            var mockResponse = "{\"result\": {\"name.family.name\": [\"James\"],\"name.given.name\": [\"Sally\"],\"name.title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(0);
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailFromResult()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(1);
            res[0].Should().Be("abba@email.uk");
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailsFromResult()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                               + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(2);
            res[0].Should().Be("abba@email.uk");
            res[1].Should().Be("test@email.uk");
        }

        [Test]
        public async Task ShouldReturnEmpltyListIfResponseIsEmptyForSuperUser()
        {
            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(0);
        }

        [Test]
        public async Task ShouldReturnEmpltyListIfResponseNotInCorrctFormatSuperUser()
        {
            
            var mockResponse = "{\"result\": {\"name.family.name\": [\"James\"],\"name.given.name\": [\"Sally\"],\"name.title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(0);
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailFromResultSuperUser()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\"],\"emails\": [\"abba@email.uk\"],\"name.givenname\": [\"Sally\"],\"Title\": [\"Miss\"]}}";
            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);
            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(1);
            res[0].Should().Be("abba@email.uk");
        }

        [Test, Ignore("Fixing")]
        public async Task ShouldReturnEmailsFromResultSuperUser()
        {
            var mockResponse = "{\"result\": {\"name.familyname\": [\"James\", \"Octavo\"],\"emails\": "
                               + "[\"abba@email.uk\", \"test@email.uk\"],\"name.givenname\": [\"Sally\", \"Chris\"],\"Title\": [\"Miss\", \"Mr\"]}}";

            _mockHttpClientWrapper.Setup(m => m.GetString(It.IsAny<string>())).ReturnsAsync(mockResponse);

            var res = await _sut.GetSuperUserEmailsAsync(10005143L);

            res.Count.ShouldBeEquivalentTo(2);
            res[0].Should().Be("abba@email.uk");
            res[1].Should().Be("test@email.uk");
        }
    }
}
