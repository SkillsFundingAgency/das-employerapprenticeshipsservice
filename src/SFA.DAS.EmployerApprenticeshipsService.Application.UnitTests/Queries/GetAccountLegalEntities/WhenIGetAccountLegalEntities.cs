using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetAccountLegalEntities
{
    public class WhenIGetAccountLegalEntities : QueryBaseTest<GetAccountLegalEntitiesQueryHandler, GetAccountLegalEntitiesRequest, GetAccountLegalEntitiesResponse>
    {
        public override GetAccountLegalEntitiesRequest Query { get; set; }
        public override GetAccountLegalEntitiesQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntitiesRequest>> RequestValidator { get; set; }

        private const long ExpectedAccountId = 123;
        private readonly string _expectedUserId = Guid.NewGuid().ToString();
        private List<LegalEntity> _legalEntities;
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
                Id = ExpectedAccountId,
                UserId = _expectedUserId
            };

            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, _expectedUserId)).ReturnsAsync(new MembershipView
            {
                RoleId = (short)Role.Owner
            });
            _employerAgreementRepository.Setup(x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId)).ReturnsAsync(_legalEntities);

        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.That(response.Entites.LegalEntityList.Count, Is.EqualTo(2));

            foreach (var legalEntity in _legalEntities)
            {
                var returned = response.Entites.LegalEntityList.SingleOrDefault(x => x.Id == legalEntity.Id);

                Assert.That(returned.Name, Is.EqualTo(legalEntity.Name));
            }
        }

        private List<LegalEntity> GetListOfLegalEntities()
        {
            return new List<LegalEntity>
            {
                new LegalEntity
                {
                    Id = 1,
                    Name = "LegalEntity1"
                },
                new LegalEntity
                {
                    Id = 2,
                    Name = "LegalEntity2"
                }
            };
        }
    }
}
