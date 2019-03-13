using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountLegalEntities
{
    public class WhenIGetAccountLegalEntities : QueryBaseTest<GetAccountLegalEntitiesQueryHandler, GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        public override GetAccountLegalEntitiesRequest Query { get; set; }
        public override GetAccountLegalEntitiesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntitiesRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedId = "123";
        private const long ExpectedAccountId = 456;
        private readonly string _expectedUserId = Guid.NewGuid().ToString();
        private List<AccountSpecificLegalEntity> _legalEntities;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _legalEntities = GetListOfLegalEntities();
            _membershipRepository = new Mock<IMembershipRepository>();
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            RequestHandler = new GetAccountLegalEntitiesQueryHandler(_membershipRepository.Object, _employerAgreementRepository.Object, RequestValidator.Object);
            Query = new GetAccountLegalEntitiesRequest
            {
                HashedLegalEntityId = ExpectedHashedId,
                UserId = _expectedUserId
            };

            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, _expectedUserId)).ReturnsAsync(new MembershipView
            {
                Role = Role.Owner,
                AccountId = ExpectedAccountId
            });
            _employerAgreementRepository.Setup(x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false)).ReturnsAsync(_legalEntities);

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
            Assert.That(response.Entites.Count, Is.EqualTo(2));

            foreach (var legalEntity in _legalEntities)
            {
                var returned = response.Entites.SingleOrDefault(x => x.Id == legalEntity.Id);

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
