using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIGetEmployerAccountsById : QueryBaseTest<GetEmployerAccountByIdHandler, GetEmployerAccountByIdQuery, GetEmployerAccountByIdResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountByIdQuery Query { get; set; }
        public override GetEmployerAccountByIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountByIdQuery>> RequestValidator { get; set; }

        private const long ExpectedAccountId = 1876;
       
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountById(ExpectedAccountId)).ReturnsAsync(new Account { Id = ExpectedAccountId });

            RequestHandler = new GetEmployerAccountByIdHandler(_employerAccountRepository.Object, RequestValidator.Object);
            Query = new GetEmployerAccountByIdQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new GetEmployerAccountByIdQuery
            {
                AccountId = ExpectedAccountId
            }, CancellationToken.None);

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountById(ExpectedAccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new GetEmployerAccountByIdQuery
            {
                AccountId = ExpectedAccountId
            }, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Account);
        }

        [Test]
        public void ThenIfValidationResponseIsUnauthorizedAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAccountByIdQuery>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async ()=> await RequestHandler.Handle(new GetEmployerAccountByIdQuery(), CancellationToken.None));
        }
    }
}
