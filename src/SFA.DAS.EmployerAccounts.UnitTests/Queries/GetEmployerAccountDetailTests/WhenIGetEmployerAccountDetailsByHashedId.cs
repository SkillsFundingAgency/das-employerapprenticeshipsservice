using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAccountDetailTests
{
    public class WhenIGetEmployerAccountDetailsByHashedId : QueryBaseTest<GetEmployerAccountDetailByHashedIdHandler, GetEmployerAccountDetailByHashedIdQuery, GetEmployerAccountDetailByHashedIdResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountDetailByHashedIdQuery Query { get; set; }
        public override GetEmployerAccountDetailByHashedIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountDetailByHashedIdQuery>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "MNBGBD";
       
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountDetailByHashedId(ExpectedHashedAccountId)).ReturnsAsync(new AccountDetail { HashedId = ExpectedHashedAccountId });

            RequestHandler = new GetEmployerAccountDetailByHashedIdHandler(RequestValidator.Object, _employerAccountRepository.Object);
            Query = new GetEmployerAccountDetailByHashedIdQuery();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new GetEmployerAccountDetailByHashedIdQuery
            {
                HashedAccountId = ExpectedHashedAccountId
            }, CancellationToken.None);

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountDetailByHashedId(ExpectedHashedAccountId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new GetEmployerAccountDetailByHashedIdQuery
            {
                HashedAccountId = ExpectedHashedAccountId
            }, CancellationToken.None);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Account);
        }
    }
}
