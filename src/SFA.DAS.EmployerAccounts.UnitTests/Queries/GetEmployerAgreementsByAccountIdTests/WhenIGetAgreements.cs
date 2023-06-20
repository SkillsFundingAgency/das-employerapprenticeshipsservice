using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementsByAccountIdTests;

internal class WhenIGetAgreements : QueryBaseTest<GetEmployerAgreementsByAccountIdRequestHandler, GetEmployerAgreementsByAccountIdRequest, GetEmployerAgreementsByAccountIdResponse>
{
    private static long AccountId => 123456;
    private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
    private List<EmployerAgreement> _agreements;

    public override GetEmployerAgreementsByAccountIdRequest Query { get; set; }
    public override GetEmployerAgreementsByAccountIdRequestHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetEmployerAgreementsByAccountIdRequest>> RequestValidator { get; set; }

    [SetUp]
    public void Arrange()
    {
        base.SetUp();

        _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
        _agreements = new List<EmployerAgreement>()
        {
            new()
            {
                Id = 1,
                SignedByName = "Test Agreement"
            }
        };

        RequestHandler = new GetEmployerAgreementsByAccountIdRequestHandler(
            _employerAgreementRepository.Object,
            RequestValidator.Object);

        Query = new GetEmployerAgreementsByAccountIdRequest
        {
            AccountId = AccountId,
        };

        _employerAgreementRepository.Setup(x => x.GetAccountAgreements(It.IsAny<long>()))
            .ReturnsAsync(_agreements);

    }
      
    [Test]
    public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {
        //Act
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _employerAgreementRepository.Verify(x => x.GetAccountAgreements(AccountId), Times.Once);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var response = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.AreEqual(_agreements, response.EmployerAgreements);
    }

    [Test]
    public void ThenShouldThrowExceptionIfRequestIsInvalid()
    {
        //Arrange
        var mockRepository = new Mock<IEmployerAgreementRepository>();
        var handler = new GetEmployerAgreementsByAccountIdRequestHandler(mockRepository.Object, new GetEmployerAgreementsByAccountIdRequestValidator());
        var query = new GetEmployerAgreementsByAccountIdRequest();

        //Assert
        Assert.ThrowsAsync<InvalidRequestException>(() => handler.Handle(query, CancellationToken.None));
    }
}