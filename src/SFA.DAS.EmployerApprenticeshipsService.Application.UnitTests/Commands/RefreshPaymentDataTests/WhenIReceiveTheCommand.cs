﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using MediatR;
using Moq;
using NLog;
using SFA.DAS.EAS.Application.Commands.Payments.RefreshPaymentData;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.Payments.Events.Api.Client;
using SFA.DAS.Payments.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshPaymentDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshPaymentDataCommandHandler _handler;
        private Mock<IValidator<RefreshPaymentDataCommand>> _validator;
        private Mock<IPaymentsEventsApiClient> _paymentsApiClient;
        private RefreshPaymentDataCommand _command;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private PageOfResults<Payment> _paymentData;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        private Provider _provider;
        private Framework _framework;
        private Standard _standard;
        
        private const string ExpectedPaymentUrl = "http://someurl";
        private const string ExpectedPeriodEnd = "R12-13";
        private const long ExpectedAccountId = 546578946;
        private const string StandardCourseName = "Standard Course";
        private const string FrameworkCourseName = "Framework Course";

        [SetUp]
        public void Arrange()
        {
            _command = new RefreshPaymentDataCommand
            {
                AccountId = ExpectedAccountId,
                PeriodEnd = ExpectedPeriodEnd,
                PaymentUrl = ExpectedPaymentUrl
            };

            _framework = new Framework
            {
                Title = FrameworkCourseName,
                FrameworkCode = 20,
                PathwayCode = 2,
                ProgrammeType = 3
            };

            _standard = new Standard
            {
                Id = "10",
                Code = 10,
                Title = StandardCourseName
            };

            _validator = new Mock<IValidator<RefreshPaymentDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshPaymentDataCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> ()});

            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>())).ReturnsAsync(null);

            _paymentData = new PageOfResults<Payment> {
                Items = new [] 
                {
                    new Payment
                    {
                        Id=Guid.NewGuid().ToString(),
                        StandardCode = _standard.Code
                    },
                    new Payment
                    {
                        Id=Guid.NewGuid().ToString(),
                        FrameworkCode = _framework.FrameworkCode,
                        PathwayCode = _framework.PathwayCode,
                        ProgrammeType = _framework.ProgrammeType
                    }
                }};
            _paymentsApiClient = new Mock<IPaymentsEventsApiClient>();
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(_paymentData);

            _mediator = new Mock<IMediator>();

            _logger = new Mock<ILogger>();

            _provider = new Provider
            {
                Name = "Test Provider"
            };

            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>()))
                .Returns(new ProvidersView
                {
                    Providers = new List<Provider> {_provider}
                });

            _apprenticeshipInfoService.Setup(x => x.GetFrameworksAsync(false))
                .ReturnsAsync(new FrameworksView
                {
                    Frameworks = new EditableList<Framework>
                    {
                       _framework
                    }
                });

            _apprenticeshipInfoService.Setup(x => x.GetStandardsAsync(false))
                .ReturnsAsync(new StandardsView
                {
                    Standards = new EditableList<Standard>
                    {
                        _standard
                    }
                });


            _handler = new RefreshPaymentDataCommandHandler(
                _validator.Object, 
                _paymentsApiClient.Object, 
                _dasLevyRepository.Object, 
                _mediator.Object, 
                _logger.Object,
                _apprenticeshipInfoService.Object);
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
        public async Task ThenThePaymentsApiIsCalledToGetPaymentData()
        {
            //Act
            await _handler.Handle(_command);
            
            //Assert
            _paymentsApiClient.Verify(x=>x.GetPayments(ExpectedPeriodEnd, ExpectedAccountId.ToString(),1));
        }


        [Test]
        public async Task ThenTheRepositoryIsNotCalledIfTheCommandIsValidAndThereAreNotPayments()
        {
            //Arrange
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new PageOfResults<Payment> {Items = new List<Payment>().ToArray()});

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x=>x.CreatePaymentData(It.IsAny<Payment>(),It.IsAny<long>(),It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledForEachPaymentIfTheCommandIsValidAndIfThereArePayments()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<Payment>(),_command.AccountId,_command.PeriodEnd, It.IsAny<string>(), It.IsAny<string>()),Times.Exactly(_paymentData.Items.Length));
        }

        [Test]
        public async Task ThenTheEventIsCalledToUpdateTheDeclarationDataWhenNewPaymentsHaveBeenCreated()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x=>x.PublishAsync(It.IsAny<ProcessPaymentEvent>()),Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledToSeeIfTheDataHasAlreadyBeenSavedAndIfItHasThenTheDataWillNotBeRefreshed()
        {
            //Arrange
            _dasLevyRepository.Setup(x => x.GetPaymentData(It.IsAny<Guid>())).ReturnsAsync(new Payment());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x=>x.GetPaymentData(It.IsAny<Guid>()), Times.Exactly(_paymentData.Items.Length));
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Never);
        }

        [Test]
        public async Task ThenWhenAnExceptionIsThrownFromTheApiClientNothingIsProcessedAndAnErrorIsLogged()
        {
            //Assert
            _paymentsApiClient.Setup(x => x.GetPayments(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new WebException());

            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.GetPaymentData(It.IsAny<Guid>()), Times.Never);
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessPaymentEvent>()), Times.Never);
            _logger.Verify(x=>x.Error(It.IsAny<WebException>(),$"Unable to get payment information for {_command.PeriodEnd} accountid {_command.AccountId}"));
        }

        [Test]
        public async Task ThenThePaymentShouldBeSavedWithTheCollectedProviderName()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<Payment>(), _command.AccountId, _command.PeriodEnd, _provider.ProviderName, It.IsAny<string>()), Times.Exactly(_paymentData.Items.Length));
        }

        [Test]
        public async Task ThenIfThereIsAPaymentForAStandardAppreticeshipTheStandardCourseNameWillBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Once);
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<Payment>(), _command.AccountId, _command.PeriodEnd, _provider.ProviderName, StandardCourseName), Times.Once);
        }

        [Test]
        public async Task ThenIfThereIsAPaymentForAFrameworkWorkAppreticeshipTheFrameworkCourseNameWillBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _apprenticeshipInfoService.Verify(x => x.GetStandardsAsync(false), Times.Once);
            _dasLevyRepository.Verify(x => x.CreatePaymentData(It.IsAny<Payment>(), _command.AccountId, _command.PeriodEnd, _provider.ProviderName, FrameworkCourseName), Times.Once);
        }
    }
}
