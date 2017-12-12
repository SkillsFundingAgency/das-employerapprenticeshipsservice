using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserByRef;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserByRefTests
{
    public class WhenIGetAUser : QueryBaseTest<GetUserByRefQueryHandler, GetUserByRefQuery, GetUserByRefResponse>
    {
        private Mock<IUserRepository> _repository;

        public override GetUserByRefQuery Query { get; set; }
        public override GetUserByRefQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserByRefQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _repository = new Mock<IUserRepository>();
          
            Query = new GetUserByRefQuery { UserRef = "ABC123" };
            RequestHandler = new GetUserByRefQueryHandler(_repository.Object, RequestValidator.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenIShouldGetNullIfTheUserCannotBeFound()
        {
            //Assign
            _repository.Setup(x => x.GetUserByRef(It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.User);
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
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
            //Assign
            var user = new User();
            _repository.Setup(x => x.GetUserByRef(It.IsAny<string>())).ReturnsAsync(user);

            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user, result.User);
            _repository.Verify(x => x.GetUserByRef(Query.UserRef), Times.Once);
        }
    }
}
