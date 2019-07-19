using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserAornLock;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserAornLockTests
{
    public class WhenIGetUserAornLockStatus : QueryBaseTest<GetUserAornLockQueryHandler, GetUserAornLockRequest, GetUserAornLockResponse>
    {
        private Mock<IUserAornPayeLockService> _userAornPayeLockService;

        public override GetUserAornLockRequest Query { get; set; }
        public override GetUserAornLockQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserAornLockRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();      

            _userAornPayeLockService = new Mock<IUserAornPayeLockService>();
            _userAornPayeLockService.Setup(x => x.UserAornPayeStatus(It.IsAny<Guid>())).ReturnsAsync(new UserAornPayeStatus());

            Query = new GetUserAornLockRequest(Guid.NewGuid().ToString());
            RequestHandler = new GetUserAornLockQueryHandler(_userAornPayeLockService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _userAornPayeLockService.Verify(x => x.UserAornPayeStatus(Query.UserRef), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_userAornPayeLockService, result.UserAornStatus);
            _userAornPayeLockService.Verify(x => x.UserAornPayeStatus(Query.UserRef), Times.Once);
        }
    }
}
