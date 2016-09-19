using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.WhileList;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.WhiteListTests
{
    class WhenIGetUserWhiteList
    {
        private Mock<ICacheProvider> _cacheProvider;
        private Mock<UserWhiteListFileBasedService> _mockUserWhiteList;
        private UserWhiteListFileBasedService _userWhiteList;
        private UserWhiteListLookUp _lookup;

        [SetUp]
        public void Arrange()
        {
            _cacheProvider = new Mock<ICacheProvider>();
            _mockUserWhiteList = new Mock<UserWhiteListFileBasedService>(_cacheProvider.Object);
            _userWhiteList = _mockUserWhiteList.Object;

            _lookup = new UserWhiteListLookUp
            {
                Emails = new List<string>
                {
                    "test@test.com"
                }
            };
        }

        [Test]
        public void ThenIShouldGetWhiteListedUserEmails()
        {
            //Assign
            _mockUserWhiteList.Setup(x => x.ReadFileByIdSync<UserWhiteListLookUp>(It.IsAny<string>()))
                .Returns(_lookup);

            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns((UserWhiteListLookUp) null);

            //Act
            var result = _userWhiteList.GetList();

            //Assert
            _mockUserWhiteList.Verify(x => x.ReadFileByIdSync<UserWhiteListLookUp>("user_white_list"), Times.Once);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Emails);
        }

        [Test]
        public void ThenIShouldReadFromCacheIfUserWhiteListHasBeenCached()
        {
            //Assign
           

            _cacheProvider.Setup(x => x.Get<UserWhiteListLookUp>(It.IsAny<string>()))
                          .Returns(_lookup);

            //Act
            var result = _userWhiteList.GetList();

            //Assert
            Assert.AreEqual(_lookup, result);
            _cacheProvider.Verify(x => x.Get<UserWhiteListLookUp>(nameof(UserWhiteListLookUp)), Times.Once);
        }
    }
}
