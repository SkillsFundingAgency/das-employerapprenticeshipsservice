using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLegalEntityById;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLegalEntityByIdTests
{
    public class WhenIGetALegalEntity : QueryBaseTest<GetLegalEntityByIdHandler, GetLegalEntityByIdQuery, GetLegalEntityByIdResponse>
    {
        private Mock<ILegalEntityRepository> _legalEntityRepository;
        public override GetLegalEntityByIdQuery Query { get; set; }
        public override GetLegalEntityByIdHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetLegalEntityByIdQuery>> RequestValidator { get; set; }

        private LegalEntityView _expectedLegalEntity;

        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _expectedLegalEntity = new LegalEntityView();

            _legalEntityRepository = new Mock<ILegalEntityRepository>();
            _legalEntityRepository.Setup(x => x.GetLegalEntityById(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(_expectedLegalEntity);
            _hashingService = new Mock<IHashingService>();

            Query = new GetLegalEntityByIdQuery
            {
                Id = 123
            };

            RequestHandler = new GetLegalEntityByIdHandler(RequestValidator.Object,_legalEntityRepository.Object, _hashingService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            var accountId = 1882;
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetLegalEntityByIdQuery>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});
            _hashingService.Setup(m => m.DecodeValue(Query.HashedAccountId)).Returns(accountId);
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _legalEntityRepository.Verify(x => x.GetLegalEntityById(accountId, Query.Id), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            RequestValidator.Setup(x => x.Validate(It.IsAny<GetLegalEntityByIdQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreSame(_expectedLegalEntity, actual.LegalEntity);
        }
    }
}
