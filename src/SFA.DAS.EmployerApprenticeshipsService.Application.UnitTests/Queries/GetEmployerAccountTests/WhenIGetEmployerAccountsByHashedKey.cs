using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIGetEmployerAccountsByHashedKey : QueryBaseTest<GetEmployerAccountHashedHandler,GetEmployerAccountHashedQuery, GetEmployerAccountResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountHashedQuery Query { get; set; }
        public override GetEmployerAccountHashedHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountHashedQuery>> RequestValidator { get; set; }
        private const string ExpectedUserId = "asdsa445";
        private const string ExpectedHashedId = "jfjfdjf444";

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountByHashedId(ExpectedHashedId)).ReturnsAsync(new Domain.Data.Entities.Account.Account());

            RequestHandler = new GetEmployerAccountHashedHandler(_employerAccountRepository.Object, RequestValidator.Object);
            Query = new GetEmployerAccountHashedQuery {HashedAccountId = ExpectedHashedId, UserId = ExpectedUserId};

        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = ExpectedHashedId,
                UserId = ExpectedUserId
            });

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountByHashedId(ExpectedHashedId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new GetEmployerAccountHashedQuery
            {
                HashedAccountId = ExpectedHashedId,
                UserId = ExpectedUserId
            });

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Account);
        }


        [Test]
        public void ThenIfValidationResponseIsUnauthorizedAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountHashedQuery>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetEmployerAccountHashedQuery()));

        }
    }
}
