using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPayeSchemeByRefTests
{
    public class WhenIGetAPayeScheme : QueryBaseTest<GetPayeSchemeByRefHandler, GetPayeSchemeByRefQuery, GetPayeSchemeByRefResponse>
    {
        private Mock<IPayeRepository> _payeRepository;
        public override GetPayeSchemeByRefQuery Query { get; set; }
        public override GetPayeSchemeByRefHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetPayeSchemeByRefQuery>> RequestValidator { get; set; }

        private PayeSchemeView _expectedPayeScheme;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _expectedPayeScheme = new PayeSchemeView();

            _payeRepository = new Mock<IPayeRepository>();
            _payeRepository.Setup(x => x.GetPayeForAccountByRef(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_expectedPayeScheme);

            Query = new GetPayeSchemeByRefQuery
            {
                HashedAccountId = "ABC123",
                Ref = "ABC/123"
            };

            RequestHandler = new GetPayeSchemeByRefHandler(RequestValidator.Object,_payeRepository.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPayeSchemeByRefQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _payeRepository.Verify(x => x.GetPayeForAccountByRef(Query.HashedAccountId, Query.Ref), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetPayeSchemeByRefQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreSame(_expectedPayeScheme, actual.PayeScheme);
        }
    }
}
