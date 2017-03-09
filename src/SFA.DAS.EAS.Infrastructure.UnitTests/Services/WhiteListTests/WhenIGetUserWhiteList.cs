using System.Collections.Generic;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.WhileList;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.WhiteListTests
{
    public class WhenIGetUserWhiteList
    {
        private Mock<ICacheProvider> _cacheProvider;
        private Mock<UserWhiteListService> _mockUserWhiteList;
        private UserWhiteListService _userWhiteList;
        private UserWhiteListLookUp _lookup;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _logger = new Mock<ILogger>();
            _mockUserWhiteList = new Mock<UserWhiteListService>(_cacheProvider.Object,_logger.Object);
            _userWhiteList = _mockUserWhiteList.Object;

            _lookup = new UserWhiteListLookUp
            {
                EmailPatterns = new List<string>
                {
                    "[a-zA-Z0-9.-]*@test.com"
                }
            };
        }

        [Test]
        public void ThenIShouldGetWhiteListedUserEmails()
        {
            //Assign
            _mockUserWhiteList.Setup(x => x.GetList()).Returns(_lookup);

            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns((UserWhiteListLookUp) null);

            //Act
            var result = _userWhiteList.IsEmailOnWhiteList("test@test.com");

            //Assert
            _mockUserWhiteList.Verify(x => x.GetList(), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenIShouldNotBeOnWhiteListIfMyEmailDoesntMatchAnyExistingPatterns()
        {
            //Assign
            _mockUserWhiteList.Setup(x => x.GetList()).Returns(_lookup);

            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns((UserWhiteListLookUp)null);

            //Act
            var result = _userWhiteList.IsEmailOnWhiteList("test@not_on_the_list.com");

            //Assert
            _mockUserWhiteList.Verify(x => x.GetList(), Times.Once);
            Assert.IsFalse(result);
        }

        [Test]
        public void ThenIfTheWhiteListIsEmptyIShouldDenyAllUsers()
        {
            //Assign

            _mockUserWhiteList.Setup(x => x.GetList()).Returns(new UserWhiteListLookUp { EmailPatterns = new List<string>() });
            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns((UserWhiteListLookUp)null);

            //Act
            var result = _userWhiteList.IsEmailOnWhiteList("test@test.com");

            //Assert
            _mockUserWhiteList.Verify(x => x.GetList(), Times.Once);
            Assert.IsFalse(result);
        }

        [Test]
        public void ThenIShouldReadFromCacheIfUserWhiteListHasBeenCached()
        {
            //Assign
            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>())).Returns(_lookup);
            _mockUserWhiteList.Setup(x => x.GetList()).CallBase();
            _mockUserWhiteList.Setup(x => x.GetDataFromStorage()).Returns(new UserWhiteListLookUp());

            //Act
            var result = _userWhiteList.IsEmailOnWhiteList("test@test.com");

            //Assert
            Assert.IsTrue(result);
            _cacheProvider.Verify(x => x.Get<UserWhiteListLookUp>(nameof(UserWhiteListLookUp)), Times.Once);
            _mockUserWhiteList.Verify(x => x.GetDataFromStorage(), Times.Never);
        }

        [Test]
        public void ThenIfIPassAnEmaptyEmailTheUserWhiteListShouldNotBeLookedUp()
        {
            //Assign
            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()));
            _mockUserWhiteList.Setup(x => x.GetDataFromStorage()).Returns(new UserWhiteListLookUp());

            //Act
            _userWhiteList.IsEmailOnWhiteList(string.Empty);

            //Assert
            _cacheProvider.Verify(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()), Times.Never);
            _mockUserWhiteList.Verify(x => x.GetDataFromStorage(), Times.Never);
        }

        [Test]
        public void ThenIShouldBeOnWhiteListIfMyEmailIsCapitalisedDifferently()
        {
            //Assign
            _mockUserWhiteList.Setup(x => x.GetList()).Returns(_lookup);

            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns((UserWhiteListLookUp)null);

            //Act
            var result = _userWhiteList.IsEmailOnWhiteList("TEST@TEST.COM");

            //Assert
            _mockUserWhiteList.Verify(x => x.GetList(), Times.Once);
            Assert.IsTrue(result);
        }
    }
}
