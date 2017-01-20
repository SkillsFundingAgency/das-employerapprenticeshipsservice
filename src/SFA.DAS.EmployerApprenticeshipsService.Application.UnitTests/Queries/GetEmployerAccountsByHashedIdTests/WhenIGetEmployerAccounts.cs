using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountsByHashedIdTests
{
    public class WhenIGetEmployerAccounts : QueryBaseTest<GetEmployerAccountsByHashedIdHandler, GetEmployerAccountsByHashedIdQuery, GetEmployerAccountsByHashedIdResponse>
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        public override GetEmployerAccountsByHashedIdQuery Query { get; set; }
        public override GetEmployerAccountsByHashedIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAccountsByHashedIdQuery>> RequestValidator { get; set; }

        private List<AccountInformation> _expectedAccounts;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _expectedAccounts = new List<AccountInformation>();

            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerAccountRepository.Setup(x => x.GetAccountsInformationByHashedId(It.IsAny<string>())).ReturnsAsync(_expectedAccounts);

            Query = new GetEmployerAccountsByHashedIdQuery
            {
                HashedAccountId = "ABC123"
            };

            RequestHandler = new GetEmployerAccountsByHashedIdHandler(RequestValidator.Object,_employerAccountRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetEmployerAccountsByHashedIdQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAccountRepository.Verify(x => x.GetAccountsInformationByHashedId(Query.HashedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetEmployerAccountsByHashedIdQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreSame(_expectedAccounts, actual.Accounts);
        }
    }
}
