using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUser;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserTest
{
    class WhenIGetAUser : QueryBaseTest<GetUserQueryHandler, GetUserQuery, GetUserResponse>
    {
        private Mock<IUserRepository> _repository;

        public override GetUserQuery Query { get; set; }
        public override GetUserQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _repository = new Mock<IUserRepository>();

            Query = new GetUserQuery { UserId = 2 };
            RequestHandler = new GetUserQueryHandler(_repository.Object, RequestValidator.Object);
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
       
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetUserById(Query.UserId), Times.Once);
        }

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
    }
}
