using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntitiesByHashedAccountId
{
    public class WhenIGetAccountLegalEntitiesByHashedAccountId : QueryBaseTest<GetAccountLegalEntitiesByHashedAccountIdQueryHandler, GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>
    {
        public override GetAccountLegalEntitiesByHashedAccountIdRequest Query { get; set; }
        public override GetAccountLegalEntitiesByHashedAccountIdQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedId = "123";
        private const long ExpectedAccountId = 456;
        private List<AccountSpecificLegalEntity> _legalEntities;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _legalEntities = GetListOfLegalEntities();
            new Mock<IMembershipRepository>();
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            Query = new GetAccountLegalEntitiesByHashedAccountIdRequest
            {
                HashedAccountId = ExpectedHashedId
            };

            _hashingService = new Mock<IHashingService>();

            long decodeAccountId = ExpectedAccountId;

            _hashingService
                .Setup(
                    m => m.TryDecodeValue(
                        ExpectedHashedId,
                        out decodeAccountId))
                .Returns(true);

            _employerAgreementRepository.Setup(
                    x => x.GetLegalEntitiesLinkedToAccount(
                        ExpectedAccountId,
                        false))
                .ReturnsAsync(_legalEntities);

            RequestHandler = new GetAccountLegalEntitiesByHashedAccountIdQueryHandler(
                _hashingService.Object,
                _employerAgreementRepository.Object,
                RequestValidator.Object);
        }

        [Test]
        public  void ThenInvalidRequestExceptionIsThrownIfHashedAccountIdIsInvalid()
        {
            long decodedAccountId;

            _hashingService
                .Setup(
                    m => m.TryDecodeValue(
                        ExpectedHashedId,
                        out decodedAccountId))
                .Returns(false);
            
            Assert.ThrowsAsync<InvalidRequestException>(
                async () =>
                    await
                RequestHandler
                    .Handle(Query));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.That(response.LegalEntities.Count, Is.EqualTo(2));

            foreach (var legalEntity in _legalEntities)
            {
                var returned = response.LegalEntities.SingleOrDefault(x => x.Id == legalEntity.Id);

                Assert.That(returned.Name, Is.EqualTo(legalEntity.Name));
            }
        }

        private List<AccountSpecificLegalEntity> GetListOfLegalEntities()
        {
            return new List<AccountSpecificLegalEntity>
            {
                new AccountSpecificLegalEntity()
                {
                    Id = 1,
                    Name = "LegalEntity1"
                    
                },
                new AccountSpecificLegalEntity()
                {
                    Id = 2,
                    Name = "LegalEntity2"
                }
            };
        }
    }
}
