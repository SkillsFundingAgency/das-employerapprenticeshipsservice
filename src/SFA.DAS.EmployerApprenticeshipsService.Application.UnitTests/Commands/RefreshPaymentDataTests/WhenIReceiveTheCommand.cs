using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshPaymentDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshPaymentDataCommandHandler _handler;
        private Mock<IValidator<RefreshPaymentDataCommand>> _validator;
        private Mock<IPaymentService> _paymentService;
        private RefreshPaymentDataCommand _command;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private PaymentDetails _paymentDetails;

        [SetUp]
        public void Arrange()
        {
            _command = new RefreshPaymentDataCommand
            {
                AccountId = 546578,
                PeriodEnd = "R12-13",
                PaymentUrl = "http://someurl"
            };

            _validator = new Mock<IValidator<RefreshPaymentDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>()))
                      .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>()))
                              .ReturnsAsync(null);

            _paymentDetails = new PaymentDetails
            {
                Id = Guid.NewGuid().ToString()
            };

            _paymentService = new Mock<IPaymentService>();
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(new List<PaymentDetails> {_paymentDetails});

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            
            _handler = new RefreshPaymentDataCommandHandler(
                _validator.Object, 
                _paymentService.Object, 
                _dasLevyRepository.Object, 
                _mediator.Object, 
                _logger.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _handler.Handle(new RefreshPaymentDataCommand());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>()), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheCommandIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>()))
                      .Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});
            
            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RefreshPaymentDataCommand()));
        }

        [Test]
        public async Task ThenThePaymentsApiIsCalledToGetPaymentData()
        {
            //Act
            await _handler.Handle(_command);
            
            //Assert
            _paymentService.Verify(x => x.GetAccountPayments(_command.PeriodEnd, _command.AccountId));
        }
        
        [Test]
        public async Task ThenTheRepositoryIsNotCalledIfTheCommandIsValidAndThereAreNotPayments()
        {
            //Arrange
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(new List<PaymentDetails>());
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<PaymentDetails>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositorySavesStandardPaymentsCorrectly()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(_paymentDetails), Times.Once);
        }
        
        [Test]
        public async Task ThenTheEventIsCalledToUpdateTheDeclarationDataWhenNewPaymentsHaveBeenCreated()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledToSeeIfTheDataHasAlreadyBeenSavedAndIfItHasThenTheDataWillNotBeRefreshed()
        {
            //Arrange
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>()))
                              .ReturnsAsync(new Payment());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetPaymentData(It.IsAny<Guid>()), Times.Once);
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenAnExceptionIsThrownFromTheApiClientNothingIsProcessedAndAnErrorIsLogged()
        {
            //Assert
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ThrowsAsync(new WebException());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetPaymentData(It.IsAny<Guid>()), Times.Never);
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Never);
            _logger.Verify(x => x.Error(It.IsAny<WebException>(),
                $"Unable to get payment information for {_command.PeriodEnd} accountid {_command.AccountId}"));
        }
    }
}
