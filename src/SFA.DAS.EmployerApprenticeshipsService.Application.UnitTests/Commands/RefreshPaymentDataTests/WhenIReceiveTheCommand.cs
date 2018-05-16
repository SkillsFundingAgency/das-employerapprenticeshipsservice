using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;
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
        private List<PaymentDetails> _paymentDetails;
        private List<Guid> _existingPaymentIds;
        private Mock<IMessagePublisher> _messagePublisher;

        [SetUp]
        public void Arrange()
        {
            _command = new RefreshPaymentDataCommand
            {
                AccountId = 546578,
                PeriodEnd = "R12-13",
                PaymentUrl = "http://someurl"
            };

            _existingPaymentIds = new List<Guid>
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            _validator = new Mock<IValidator<RefreshPaymentDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>()))
                      .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetAccountPaymentIds(It.IsAny<long>()))
                .ReturnsAsync(new HashSet<Guid>(_existingPaymentIds));

            _paymentDetails = new List<PaymentDetails>{ new PaymentDetails
            {
                Id = Guid.NewGuid().ToString(),
                Amount = 1234,
                EmployerAccountId = 123,
                ProviderName = "Test Learning Ltd"
            }};

            _paymentService = new Mock<IPaymentService>();
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(_paymentDetails);

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _messagePublisher = new Mock<IMessagePublisher>();
            
            _handler = new RefreshPaymentDataCommandHandler(
                _messagePublisher.Object,
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
            _dasLevyRepository.Verify(x => x.CreatePayments(It.IsAny<IEnumerable<PaymentDetails>>()), Times.Never);
        }

        [Test]
        public async Task ThenTheRepositorySavesStandardPaymentsCorrectly()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePayments(_paymentDetails), Times.Once);
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
            _paymentDetails = new List<PaymentDetails>
            {
                new PaymentDetails { Id = _existingPaymentIds[0].ToString().ToLower()},
                new PaymentDetails { Id = _existingPaymentIds[1].ToString().ToUpper()}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(_paymentDetails);

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePayments(It.IsAny<IEnumerable<PaymentDetails>>()), Times.Never);
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
            _dasLevyRepository.Verify(x => x.GetAccountPaymentIds(_command.AccountId), Times.Never);
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Never);

            _logger.Verify(x => x.Error(It.IsAny<WebException>(),
                $"Unable to get payment information for AccountId = '{_command.AccountId}' and PeriodEnd = '{_command.PeriodEnd}'"));
        }

        [Test]
        public async Task ShouldGetExistingPaymentIdsFromDatabase()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetAccountPaymentIds(_command.AccountId), Times.Once);
        }

        [Test]
        public async Task ShouldOnlyAddPaymentsWhichAreNotAlreadyAdded()
        {
            //Arrange
            var newPaymentGuid = Guid.NewGuid();
            _paymentDetails = new List<PaymentDetails>
            {
                new PaymentDetails { Id = _existingPaymentIds[0].ToString()},
                new PaymentDetails { Id = _existingPaymentIds[1].ToString()},
                new PaymentDetails { Id = newPaymentGuid.ToString()}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(_paymentDetails);

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePayments(It.Is<IEnumerable<PaymentDetails>>(s => 
                s.Any(p => p.Id.Equals(newPaymentGuid.ToString())) &&
                s.Count() == 1)));

            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Once);
        }
        
        [Test]
        public async Task ThenAnPaymentCreatedMessageIsCreated()
        {
            //Arrange
            var expectedPayment = _paymentDetails.First();

            //Act
            await _handler.Handle(new RefreshPaymentDataCommand());

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<PaymentCreatedMessage>
                (m => m.Amount.Equals(expectedPayment.Amount) &&
                m.ProviderName.Equals(expectedPayment.ProviderName))), Times.Once);
        }

        [Test]
        public void ThenAnAccountPaymentCreatedIsNotCreatedIfPaymentProcessingFails()
        {
            //Arrange
            _dasLevyRepository.Setup(x => x.CreatePayments(It.IsAny<IEnumerable<PaymentDetails>>()))
                .Throws<Exception>();

            //Act
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(new RefreshPaymentDataCommand()));

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<PaymentCreatedMessage>()), Times.Never());
        }

    }
}
