using System.Web;
using Moq;
using NUnit.Framework;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.CookieStorageTests
{
    public class WhenIUseTheCookieStorageService
    {
        private CookieStorageService<TestStorageClass> _cookieStorageService;
        private Mock<ICookieService<TestStorageClass>> _cookieService;
        private Mock<HttpContextBase> _httpContextBase;
        public const string ExpectedCookieName = "CookieTestName";

        [SetUp]
        public void Arrange()
        {
            _cookieService = new Mock<ICookieService<TestStorageClass>>();

            _httpContextBase = new Mock<HttpContextBase>();

            _cookieStorageService = new CookieStorageService<TestStorageClass>(_cookieService.Object, _httpContextBase.Object);
        }

        [Test]
        public void ThenTheInformationIsStoredByCallingTheCookieService()
        {
            //Arrange
            var expectedExpiryDays = 1;

            //Act
            _cookieStorageService.Create(new TestStorageClass(), ExpectedCookieName, expectedExpiryDays);

            //Assert
            _cookieService.Verify(x=>x.Create(It.IsAny<HttpContextBase>(), ExpectedCookieName, It.IsAny<TestStorageClass>(), expectedExpiryDays));
        }

        [Test]
        public void ThenTheInformationIsReadFromTheCookieService()
        {
            //Arrange
            _cookieService.Setup(x => x.Get(It.IsAny<HttpContextBase>(), ExpectedCookieName)).Returns(new TestStorageClass());

            //Act
            var actual = _cookieStorageService.Get(ExpectedCookieName);

            //Assert
            Assert.IsAssignableFrom<TestStorageClass>(actual);
        }

        [Test]
        public void ThenTheCookieIsDeletedWhenCalledByName()
        {
            //Act
            _cookieStorageService.Delete(ExpectedCookieName);

            //Assert
            _cookieService.Verify(x=>x.Delete(It.IsAny<HttpContextBase>(), ExpectedCookieName));
        }

        [Test]
        public void ThenTheCookieIsUpdatedIfItExists()
        {
            //Act
            _cookieStorageService.Update(ExpectedCookieName, new TestStorageClass());

            //Assert
            _cookieService.Verify(x=>x.Update(It.IsAny<HttpContextBase>(),ExpectedCookieName,It.IsAny<TestStorageClass>()));
        }

        public class TestStorageClass
        {
            
        }
    }
}
