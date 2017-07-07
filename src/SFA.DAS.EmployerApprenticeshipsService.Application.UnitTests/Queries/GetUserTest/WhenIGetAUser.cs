using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUser;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserTest
{
    public class WhenIGetAUser : QueryBaseTest<GetUserQueryHandler, GetUserQuery, GetUserResponse>
    {
        private Mock<IUserRepository> _repository;
        private Mock<IHashingService> _hashingService;

        public override GetUserQuery Query { get; set; }
        public override GetUserQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _repository = new Mock<IUserRepository>();
            _hashingService = new Mock<IHashingService>();

            Query = new GetUserQuery { UserId = 2 };
            RequestHandler = new GetUserQueryHandler(_repository.Object, _hashingService.Object, RequestValidator.Object);
        }

        [Test]
        public async Task ThenIShouldGetNullIfTheUserCannotBeFound()
        {
            //Assign
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.User);
            _repository.Verify(x => x.GetUserById(Query.UserId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetUserById(Query.UserId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Assign
            var user = new User();
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(user);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user, result.User);
            _repository.Verify(x => x.GetUserById(Query.UserId), Times.Once);
        }

        [Test]
        public async Task ThenIShouldGetTheUserIdFromTheHashingServiceIfAHashedIdIsProvided()
        {
            //Assign
            const int userId = 34;
            Query.UserId = 0;
            Query.HashedUserId = "ABC123";
            _hashingService.Setup(x => x.DecodeValue(Query.HashedUserId)).Returns(userId);
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(new User());

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hashingService.Verify(x => x.DecodeValue(Query.HashedUserId), Times.Once);
            _repository.Verify(x => x.GetUserById(userId), Times.Once);
        }

        [Test]
        public async Task ThenIShouldNotCallTheHashingServiceIfTheUserIdIsProvided()
        {
            //Assign
            _repository.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(new User());

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hashingService.Verify(x => x.DecodeValue(It.IsAny<string>()), Times.Never);
        }
    }
}
