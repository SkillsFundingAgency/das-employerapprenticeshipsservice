using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Reservations;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetReservations;

public class WhenIGetReservations : QueryBaseTest<GetReservationsRequestHandler, GetReservationsRequest, GetReservationsResponse>
{
    public override GetReservationsRequest Query { get; set; }
    public override GetReservationsRequestHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetReservationsRequest>> RequestValidator { get; set; }

    private Mock<IReservationsService> _reservationsService;
    private Reservation _reservation;
    private Mock<ILogger<GetReservationsRequestHandler>> _logger;
    private long _accountId;


    [SetUp]
    public void Arrange()
    {
        SetUp();

        _accountId = 123;

        _reservation = new Reservation();
        _logger = new Mock<ILogger<GetReservationsRequestHandler>>();

        _reservationsService = new Mock<IReservationsService>();
        _reservationsService
            .Setup(s => s.Get(_accountId))
            .ReturnsAsync(new List<Reservation> { _reservation });

        RequestHandler = new GetReservationsRequestHandler(RequestValidator.Object, _logger.Object, _reservationsService.Object);

        Query = new GetReservationsRequest
        {
            AccountId = _accountId
        };
    }

    [Test]
    public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
    {
        //Act
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _reservationsService.Verify(x => x.Get(_accountId), Times.Once);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var response = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.Contains(_reservation, (ICollection)response.Reservations);
    }

    public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {
        throw new System.NotImplementedException();
    }
}