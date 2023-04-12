using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntitiesByHashedAccountId;

public class WhenIGetAccountLegalEntitiesByHashedAccountId : QueryBaseTest<GetAccountLegalEntitiesByHashedAccountIdQueryHandler, GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>
{
    public override GetAccountLegalEntitiesByHashedAccountIdRequest Query { get; set; }
    public override GetAccountLegalEntitiesByHashedAccountIdQueryHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest>> RequestValidator { get; set; }

    private const string ExpectedHashedId = "123";
    private List<AccountLegalEntity> _legalEntities;
    private Mock<IAccountLegalEntityRepository> _accountLegalEntityRepository;
        
    [SetUp]
    public void Arrange()
    {
        base.SetUp();

        _legalEntities = GetListOfLegalEntities();
        _accountLegalEntityRepository = new Mock<IAccountLegalEntityRepository>();

        Query = new GetAccountLegalEntitiesByHashedAccountIdRequest
        {
            HashedAccountId = ExpectedHashedId
        };

        _accountLegalEntityRepository.Setup(
                x => x.GetAccountLegalEntities(
                    ExpectedHashedId))
            .ReturnsAsync(_legalEntities);

        RequestHandler = new GetAccountLegalEntitiesByHashedAccountIdQueryHandler(_accountLegalEntityRepository.Object, RequestValidator.Object);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {
        //Act
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _accountLegalEntityRepository.Verify(x => x.GetAccountLegalEntities(ExpectedHashedId), Times.Once);
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

    private static List<AccountLegalEntity> GetListOfLegalEntities()
    {
        return new List<AccountLegalEntity>
        {
            new AccountLegalEntity()
            {
                Id = 1,
                Name = "LegalEntity1"
                    
            },
            new AccountLegalEntity()
            {
                Id = 2,
                Name = "LegalEntity2"
            }
        };
    }
}