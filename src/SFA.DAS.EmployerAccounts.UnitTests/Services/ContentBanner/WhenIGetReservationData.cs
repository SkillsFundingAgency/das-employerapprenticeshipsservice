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
        private Mock<IContentBannerApiClient> _mockContentBannerApiClient;
        private IContentBannerService _sut;
        private string _testData;
        private bool _useCDN;
        int _bannerId;

        [SetUp]
        public void Arrange()
        {
            _bannerId = 123;
            _testData = "<h1>My First Heading</h1>" +
                        "<p>My first paragraph.</p>";

            _mockContentBannerApiClient = new Mock<IContentBannerApiClient>();
            _mockContentBannerApiClient
                .Setup(m => m.GetBanner(_bannerId, _useCDN))
                .ReturnsAsync(_testData);

            _sut = new ContentBannerService(_mockContentBannerApiClient.Object);
        }

        [Test]
        public async Task ThenTheReservationsAreReturnedFromTheApi()
        {
            // arrange

            // act
            var result = await _sut.GetBannerContent(_bannerId,_useCDN);

            // assert            
            result.Should().Be(_testData);
        }
    }
}
