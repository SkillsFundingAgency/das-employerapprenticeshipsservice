using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountByHashedId;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountByHashedIdTests
{
    public class WhenIGetEmployerAccounts : QueryBaseTest<GetEmployerAccountByHashedIdHandler, GetEmployerAccountByHashedIdQuery, GetEmployerAccountByHashedIdResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountByHashedIdQuery Query { get; set; }
        public override GetEmployerAccountByHashedIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountByHashedIdQuery>> RequestValidator { get; set; }

        private AccountDetail _expectedAccount;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _expectedAccount = new AccountDetail();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountDetailByHashedId(It.IsAny<string>())).ReturnsAsync(_expectedAccount);

            Query = new GetEmployerAccountByHashedIdQuery
            {
                HashedAccountId = "ABC123"
            };

            RequestHandler = new GetEmployerAccountByHashedIdHandler(RequestValidator.Object,_employerAccountRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetEmployerAccountByHashedIdQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountDetailByHashedId(Query.HashedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetEmployerAccountByHashedIdQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreSame(_expectedAccount, actual.Account);
        }
    }
}
