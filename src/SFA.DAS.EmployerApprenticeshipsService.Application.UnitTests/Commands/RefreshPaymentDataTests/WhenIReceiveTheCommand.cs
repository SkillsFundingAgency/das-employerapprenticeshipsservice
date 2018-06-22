using FluentAssertions;
using MediatR;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Provider.Events.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshPaymentDataTests
{
    public class WhenIReceiveTheCommand
    {
        private const long AccountId = 10;
        private const decimal Amount = 145.67M;
        private const string ProviderName = "Test Learning Ltd";


        private RefreshPaymentDataCommandHandler _handler;
        private Mock<IValidator<RefreshPaymentDataCommand>> _validator;
        private Mock<IPaymentService> _paymentService;
        private RefreshPaymentDataCommand _command;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private List<PaymentDetails> _paymentDetails;
        private List<Guid> _existingPaymentIds;
        private TestableEndpointInstance _endpoint;

        [SetUp]
        public void Arrange()
        {
            _command = new RefreshPaymentDataCommand
            {
                AccountId = AccountId,
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
                      .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetAccountPaymentIds(It.IsAny<long>()))
                .ReturnsAsync(new HashSet<Guid>(_existingPaymentIds));

            _paymentDetails = new List<PaymentDetails>{ new PaymentDetails
            {
                Id = Guid.NewGuid(),
                Amount = Amount,
                EmployerAccountId = AccountId,
                ProviderName = ProviderName
            }};

            _paymentService = new Mock<IPaymentService>();
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(_paymentDetails);

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _endpoint = new TestableEndpointInstance();

            _handler = new RefreshPaymentDataCommandHandler(
                _endpoint,
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
                      .Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

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
                new PaymentDetails { Id = _existingPaymentIds[0]},
                new PaymentDetails { Id = _existingPaymentIds[1]}
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
                $"Unable to get payment information for AccountId = '{AccountId}' and PeriodEnd = '{_command.PeriodEnd}'"));
        }

        [Test]
        public async Task ShouldGetExistingPaymentIdsFromDatabase()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetAccountPaymentIds(AccountId), Times.Once);
        }

        [Test]
        public async Task ShouldOnlyAddPaymentsWhichAreNotAlreadyAdded()
        {
            //Arrange
            var newPaymentGuid = Guid.NewGuid();
            _paymentDetails = new List<PaymentDetails>
            {
                new PaymentDetails { Id = _existingPaymentIds[0]},
                new PaymentDetails { Id = _existingPaymentIds[1]},
                new PaymentDetails { Id = newPaymentGuid}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                           .ReturnsAsync(_paymentDetails);

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePayments(It.Is<IEnumerable<PaymentDetails>>(s =>
                s.Any(p => p.Id.Equals(newPaymentGuid)) &&
                s.Count() == 1)));

            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Once);
        }

        [Test]
        public async Task ShouldOnlyAddNonFullyFundedPayments()
        {
            //Arrange
            var newPaymentGuid = Guid.NewGuid();
            var fullyFundedPaymentGuid = Guid.NewGuid();
            _paymentDetails = new List<PaymentDetails>
            {
                new PaymentDetails { Id = newPaymentGuid, FundingSource = FundingSource.Levy},
                new PaymentDetails { Id = fullyFundedPaymentGuid, FundingSource = FundingSource.FullyFundedSfa}
            };

            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(_paymentDetails);

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePayments(It.Is<IEnumerable<PaymentDetails>>(s =>
                s.All(p => !p.Id.Equals(fullyFundedPaymentGuid)) &&
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
            _endpoint.PublishedMessages.Should().HaveCount(1);

            var message = _endpoint.PublishedMessages.Select(x => x.Message)
                                                     .OfType<CreatedPaymentEvent>()
                                                     .Single();
            message.AccountId.Should().Be(AccountId);
            message.Amount.Should().Be(Amount);
            message.ProviderName.Should().Be(ProviderName);
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
            _endpoint.PublishedMessages.Should().BeEmpty();
        }
    }
}
