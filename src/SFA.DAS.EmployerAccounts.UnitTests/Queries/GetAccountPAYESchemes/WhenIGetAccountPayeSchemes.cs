using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Levy;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountPAYESchemes;

class WhenIGetAccountPayeSchemes : QueryBaseTest<GetAccountPayeSchemesQueryHandler, GetAccountPayeSchemesQuery, GetAccountPayeSchemesResponse>
{
    private const long AccountId = 2;
    private static readonly DateTime UpdateDate = DateTime.Now;

    private PayeView _payeView;

    private Mock<IPayeSchemesService> _payeSchemesService;
    private long _accountId;

    public override GetAccountPayeSchemesQuery Query { get; set; }
    public override GetAccountPayeSchemesQueryHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetAccountPayeSchemesQuery>> RequestValidator { get; set; }
       

    [SetUp]
    public void Arrange()
    {
        SetUp();

        _payeView = new PayeView
        {
            AccountId = AccountId,
            Ref = "123/ABC"
        };

        _accountId = 1876;
        Query = new GetAccountPayeSchemesQuery()
        {
            AccountId = _accountId,
        };

        _payeSchemesService = new Mock<IPayeSchemesService>();

        _payeSchemesService
            .Setup(
                m => m.GetPayeSchemes(_accountId)
            )
            .ReturnsAsync(new List<PayeView> {_payeView});


        RequestHandler = new GetAccountPayeSchemesQueryHandler(
            _payeSchemesService.Object,
            RequestValidator.Object);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {
        //Act
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _payeSchemesService.Verify(x => x.GetPayeSchemes(_accountId), Times.Once);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var result = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.AreEqual(1, result.PayeSchemes.Count);
        Assert.AreEqual(_payeView, result.PayeSchemes.First());
    }
}