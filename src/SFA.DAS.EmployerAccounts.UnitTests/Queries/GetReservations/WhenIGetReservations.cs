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
using SFA.DAS.EmployerAccounts.Validation;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetReservations
{
    public class WhenIGetReservations : QueryBaseTest<GetReservationsRequestHandler, GetReservationsRequest, GetReservationsResponse>
    {
        public override GetReservationsRequest Query { get; set; }
        public override GetReservationsRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetReservationsRequest>> RequestValidator { get; set; }

        private Mock<IReservationsService> _reservationsService;
        private Mock<IHashingService> _hashingService;
        private Reservation _reservation;
        private Mock<ILogger<GetReservationsRequestHandler>> _logger;
        private string _hashedAccountId;
        private long _accountId;

        
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _hashedAccountId = "123ABC";
            _accountId = 123;

            _reservation = new Reservation();
            _logger = new Mock<ILogger<GetReservationsRequestHandler>>();

            _reservationsService = new Mock<IReservationsService>();
            _reservationsService
                .Setup(s => s.Get(_accountId))
                .ReturnsAsync(new List<Reservation> { _reservation });

            _hashingService = new Mock<IHashingService>();
            _hashingService
                .Setup(m => m.DecodeValue(_hashedAccountId))
                .Returns(_accountId);
            
            RequestHandler = new GetReservationsRequestHandler(RequestValidator.Object, _logger.Object, _reservationsService.Object, _hashingService.Object);

            Query = new GetReservationsRequest
            {
                AccountId = _hashedAccountId
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
        public async Task ThenIfTheMessageIsValidTheHashingServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _hashingService.Verify(x => x.DecodeValue(_hashedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.Contains(_reservation, (ICollection) response.Reservations);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new System.NotImplementedException();
        }
    }
}
