using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserAornLockTests
{
    public class WhenIGetUserAornLockStatus
    {
        private Mock<IUserAornPayeLockService> _userAornPayeLockService;
        public GetUserAornLockRequest Query { get; set; }
        public GetUserAornLockQueryHandler RequestHandler { get; set; }
       
        [SetUp]
        public void Arrange()
        {
            _userAornPayeLockService = new Mock<IUserAornPayeLockService>();
            _userAornPayeLockService.Setup(x => x.UserAornPayeStatus(It.IsAny<Guid>())).ReturnsAsync(new UserAornPayeStatus());

            Query = new GetUserAornLockRequest(Guid.NewGuid().ToString());
            RequestHandler = new GetUserAornLockQueryHandler(_userAornPayeLockService.Object);
        }

        [Test]
        public async Task ThenIfTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _userAornPayeLockService.Verify(x => x.UserAornPayeStatus(Query.UserRef), Times.Once);
        }

        [Test]
        public async Task ThenTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            _userAornPayeLockService.Verify(x => x.UserAornPayeStatus(Query.UserRef), Times.Once);
        }
    }
}
