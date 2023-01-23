using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIGetEmployerAccountsByHashedId : QueryBaseTest<GetEmployerAccountByHashedIdHandler, GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountByHashedIdQuery Query { get; set; }
        public override GetEmployerAccountByHashedIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountByHashedIdQuery>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "MNBGBD";
       
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountByHashedId(ExpectedHashedAccountId)).ReturnsAsync(new Account { HashedId = ExpectedHashedAccountId });

            RequestHandler = new GetEmployerAccountByHashedIdHandler(_employerAccountRepository.Object, RequestValidator.Object);
            Query = new GetEmployerAccountByHashedIdQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = ExpectedHashedAccountId
            }, CancellationToken.None);

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountByHashedId(ExpectedHashedAccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = ExpectedHashedAccountId
            }, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Account);
        }

        [Test]
        public void ThenIfValidationResponseIsUnauthorizedAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountByHashedIdQuery>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async ()=> await RequestHandler.Handle(new GetEmployerAccountByHashedIdQuery(), CancellationToken.None));
        }
    }
}
