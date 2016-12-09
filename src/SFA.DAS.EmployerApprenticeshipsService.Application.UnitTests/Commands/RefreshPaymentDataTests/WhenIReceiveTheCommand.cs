using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using MediatR;
using Moq;
using NLog;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshPaymentDataTests
{
    public class WhenIReceiveTheCommand
    {
        private const string ExpectedPaymentUrl = "http://someurl";
        private const string ExpectedPeriodEnd = "R12-13";
        private const long ExpectedAccountId = 546578946;
       
        private Mock<IValidator<RefreshPaymentDataCommand>> _validator;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IPaymentService> _paymentService;

        private RefreshPaymentDataCommandHandler _handler;
        private PaymentDetails _paymentDetails;
        private RefreshPaymentDataCommand _command;

        [SetUp]
        public void Arrange()
        {
            _command = new RefreshPaymentDataCommand
            {
                AccountId = ExpectedAccountId,
                PeriodEnd = ExpectedPeriodEnd,
                PaymentUrl = ExpectedPaymentUrl
            };

            _paymentDetails = new PaymentDetails()
            {
                Id = Guid.NewGuid().ToString()
            };

            _validator = new Mock<IValidator<RefreshPaymentDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>())).ReturnsAsync(null);

            _paymentService = new Mock<IPaymentService>();
            _paymentService.Setup(x => x.GetAccountPayments(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentDetails>
                {
                   _paymentDetails
                });

            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
          
            _handler = new RefreshPaymentDataCommandHandler(
                _validator.Object,
                _dasLevyRepository.Object,
                _paymentService.Object,
                _mediator.Object, 
                _logger.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _handler.Handle(new RefreshPaymentDataCommand());

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<RefreshPaymentDataCommand>()),Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheCommandIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});
            
            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(new RefreshPaymentDataCommand()));
        }
        
        [Test]
        public async Task ThenTheRepositoryIsNotCalledIfTheCommandIsValidAndThereAreNotPayments()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<PaymentDetails>()), Times.Once());
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledForEachPaymentIfTheCommandIsValidAndIfThereArePayments()
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
        public async Task ThenTheRepositoryIsCalledToSeeIfThePaymentAlreadyExists()
        {
            //Arrange
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>())).ReturnsAsync(new Payment());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetPaymentData(Guid.Parse(_paymentDetails.Id)), Times.Once);
        }

        [Test]
        public async Task ThenIShouldGetPaymentDetailsFromThePaymentService()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _paymentService.Verify(x => x.GetAccountPayments(ExpectedPeriodEnd, ExpectedAccountId.ToString()), Times.Once);
        }

        [Test]
        public async Task ThenThePaymentShouldNotBeSavedIfPaymentAlreadyExists()
        {
            //Arrange
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>())).ReturnsAsync(new Payment());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<PaymentDetails>()), Times.Never);
        }

        [Test]
        public async Task ThenThePaymentShouldBeSavedIfPaymentDoesNotExist()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(_paymentDetails), Times.Once);
        }
    }
}
