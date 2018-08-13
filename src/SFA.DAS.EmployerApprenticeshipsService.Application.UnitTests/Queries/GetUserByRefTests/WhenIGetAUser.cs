using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Queries.GetUserByRef;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserByRefTests
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
            RequestHandler = new GetUserByRefQueryHandler(_repository.Object, RequestValidator.Object, Mock.Of<ILog>());
        }

        [Test]
        public void ThenIShouldThrowExceptionIfTheUserCannotBeFound()
        {
            //Assign
            _repository.Setup(x => x.GetUserByRef(It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () =>
            {
                await RequestHandler.Handle(Query);
            });
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_user, result.User);
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
        }
    }
}
