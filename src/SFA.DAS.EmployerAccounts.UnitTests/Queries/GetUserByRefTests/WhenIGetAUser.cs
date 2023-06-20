using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByRef;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserByRefTests
{
    public class WhenIGetAUser : QueryBaseTest<GetUserByRefQueryHandler, GetUserByRefQuery, GetUserByRefResponse>
    {
        private Mock<IUserRepository> _repository;
        private User _user;

        public override GetUserByRefQuery Query { get; set; }
        public override GetUserByRefQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserByRefQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _user = new User();

            _repository = new Mock<IUserRepository>();

            _repository.Setup(x => x.GetUserByRef(It.IsAny<string>())).ReturnsAsync(_user);

            Query = new GetUserByRefQuery { UserRef = "ABC123" };
            RequestHandler = new GetUserByRefQueryHandler(_repository.Object, RequestValidator.Object, Mock.Of<ILogger<GetUserByRefQueryHandler>>());
        }

        [Test]
        public void ThenIShouldThrowExceptionIfTheUserCannotBeFound()
        {
            //Assign
            _repository.Setup(x => x.GetUserByRef(It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () =>
            {
                await RequestHandler.Handle(Query, CancellationToken.None);
            });
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_user, result.User);
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
        }
    }
}
