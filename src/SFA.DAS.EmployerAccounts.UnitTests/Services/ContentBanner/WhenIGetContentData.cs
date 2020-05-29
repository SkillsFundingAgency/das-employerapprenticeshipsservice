using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.ContentBanner
{
    class WhenIGetContentBanner
    {
        private Mock<IClientContentApiClient> _mockContentBannerApiClient;
        private IClientContentService _sut;
        private string _testData;
        
        [SetUp]
        public void Arrange()
        {
            _testData = "<h1>My First Heading</h1>" +
                        "<p>My first paragraph.</p>";

            _mockContentBannerApiClient = new Mock<IClientContentApiClient>();
            _mockContentBannerApiClient
                .Setup(m => m.Get("banner","eas-acc"))
                .ReturnsAsync(_testData);

            _sut = new ClientContentService(_mockContentBannerApiClient.Object);
        }

        [Test]
        public async Task ThenThContentAreReturnedFromTheApi()
        {
            // arrange

            // act
            var result = await _sut.Get("banner", "eas-acc");

            // assert            
            result.Should().Be(_testData);
        }
    }
}
