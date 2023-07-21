using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.UpdateUserAornLock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.UpdateUserAornLockTests
{
    public class WhenIUpdateUserAornLockStatus 
    {
        private Mock<IUserAornPayeLockService> _userAornPayeLockService;
        public UpdateUserAornLockRequest Query { get; set; }
        public UpdateUserAornLockQueryHandler RequestHandler { get; set; }
       
        [SetUp]
        public void Arrange()
        {
            _userAornPayeLockService = new Mock<IUserAornPayeLockService>();
            _userAornPayeLockService.Setup(x => x.UpdateUserAornPayeAttempt("UserRef", true)).ReturnsAsync(true);

            Query = new UpdateUserAornLockRequest
            {
                UserRef = "UserRef",
                Success = true
            };

            RequestHandler = new UpdateUserAornLockQueryHandler(_userAornPayeLockService.Object);
        }

        [Test]
        public async Task ThenThePayeAttemptIsUpdated()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _userAornPayeLockService.Verify(x => x.UpdateUserAornPayeAttempt(Query.UserRef, Query.Success), Times.Once);
        }
    }
}
