using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntities;

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
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _employerAgreementRepository.Verify(x => x.GetLegalEntitiesLinkedToAccount(ExpectedAccountId, false), Times.Once);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var response = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.That(response.LegalEntities.Count, Is.EqualTo(2));

        foreach (var legalEntity in _legalEntities)
        {
            var returned = response.LegalEntities.SingleOrDefault(x => x.Id == legalEntity.Id);

            Assert.That(returned.Name, Is.EqualTo(legalEntity.Name));
        }
    }

    private static List<AccountSpecificLegalEntity> GetListOfLegalEntities()
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