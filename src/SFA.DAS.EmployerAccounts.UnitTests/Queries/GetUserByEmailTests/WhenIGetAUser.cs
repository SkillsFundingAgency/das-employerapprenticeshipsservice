﻿using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Queries.GetUserByEmail;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUserByEmailTests
{
    public class WhenIGetAUser : QueryBaseTest<GetUserByEmailQueryHandler, GetUserByEmailQuery, GetUserByEmailResponse>
    {
        private Mock<IUserAccountRepository> _repository;
        private User _user;

        public override GetUserByEmailQuery Query { get; set; }
        public override GetUserByEmailQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetUserByEmailQuery>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _user = new User();

            _repository = new Mock<IUserAccountRepository>();

            _repository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(_user);

            Query = new GetUserByEmailQuery { Email = "test@test.com" };
            RequestHandler = new GetUserByEmailQueryHandler(_repository.Object, RequestValidator.Object);
        }

        [Test]
        public async Task ThenIShouldThrowExceptionIfTheUserCannotBeFound()
        {
            //Assign
            _repository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            var result = await RequestHandler.Handle(Query);
            Assert.IsNotNull(result);
            Assert.IsNull(result.User);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.Get(Query.Email), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_user, result.User);
            _repository.Verify(x => x.Get(Query.Email), Times.Once);
        }
    }
}
