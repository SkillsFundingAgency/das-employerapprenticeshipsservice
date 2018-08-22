﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Levy;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Commands.PublishGenericEvent;
using SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.EmployerFinance.Exceptions;
using SFA.DAS.EmployerFinance.Factories;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.UnitTests.ObjectMothers;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    public class WhenIReceiveTheCommand
    {
        private RefreshEmployerLevyDataCommandHandler _refreshEmployerLevyDataCommandHandler;
        private Mock<IValidator<RefreshEmployerLevyDataCommand>> _validator;
        private Mock<IDasLevyRepository> _levyRepository;
        private Mock<IMediator> _mediator;
        private Mock<IHmrcDateService> _hmrcDateService;
        private Mock<ILevyEventFactory> _levyEventFactory;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private Mock<IHashingService> _hashingService;
        private Mock<ILog> _logger;
        private const string ExpectedEmpRef = "123456";
        private const long ExpectedAccountId = 44321;

        [SetUp]
        public void Arrange()
        {
            _levyRepository = new Mock<IDasLevyRepository>();
            _levyRepository.Setup(x => x.GetLastSubmissionForScheme(ExpectedEmpRef)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 1000m, LevyAllowanceForFullYear = 1200m });

            _validator = new Mock<IValidator<RefreshEmployerLevyDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());

            _mediator = new Mock<IMediator>();

            _hmrcDateService = new Mock<IHmrcDateService>();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<DateTime>())).Returns(false);

            _levyEventFactory = new Mock<ILevyEventFactory>();
            _genericEventFactory = new Mock<IGenericEventFactory>();
            _hashingService = new Mock<IHashingService>();
            _logger = new Mock<ILog>();

            _refreshEmployerLevyDataCommandHandler = new RefreshEmployerLevyDataCommandHandler(_validator.Object, _levyRepository.Object, _mediator.Object, _hmrcDateService.Object,
                _levyEventFactory.Object, _genericEventFactory.Object, _hashingService.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheValidatorIsCalled()
        {
            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef));

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>()));
        }

        [Test]
        public void ThenAInvalidRequestExceptionIsThrownIfTheMessageIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _refreshEmployerLevyDataCommandHandler.Handle(new RefreshEmployerLevyDataCommand()));
        }

        [Test]
        public async Task ThenAnyDuplicateSubmissionIdsFromHmrcAreFiltered()
        {
            //Arrange 
            var refreshEmployerLevyDataCommand =
                RefreshEmployerLevyDataCommandObjectMother.CreateDuplicateHmrcSubmissions(ExpectedEmpRef,
                    ExpectedAccountId);

            //Act 
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert 
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(
                It.Is<IEnumerable<DasDeclaration>>(d => d.Count() == 1),
                It.Is<string>(s => s == ExpectedEmpRef),
                It.Is<long>(a => a == ExpectedAccountId)
            ), Times.Once());
        }

        [Test]
        public async Task ThenAnyDuplicateSubmissionIdsFromHmrcThatAreFilteredAreLogged()
        {
            //Arrange 
            var refreshEmployerLevyDataCommand =
                RefreshEmployerLevyDataCommandObjectMother.CreateDuplicateHmrcSubmissions(ExpectedEmpRef,
                    ExpectedAccountId);

            //Act 
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            ////Assert 
            _logger.Verify(x => x.Info(It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task ThenTheExistingDeclarationIdsAreCollected()
        {
            //Arrange
            var refreshEmployerLevyDataCommand = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert
            _levyRepository.Verify(x => x.GetEmployerDeclarationSubmissionIds(ExpectedEmpRef), Times.Once());
        }

        [Test]
        public async Task ThenTheLevyRepositoryIsUpdatedIfTheDeclarationDoesNotExist()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclarationSubmissionIds(ExpectedEmpRef)).ReturnsAsync(new List<long> { 2 });

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId));

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.IsAny<IEnumerable<DasDeclaration>>(), ExpectedEmpRef, ExpectedAccountId));
        }

        [Test]
        public async Task ThenIfThereAreDeclarationsToProcessTheProcessDeclarationsEventIsPublished()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessDeclarationsEvent>()), Times.Once);
        }

        [Test]
        public async Task ThenIfThereAreNoNewDeclarationsThenTheProcessDeclarationEventIsNotPublished()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclarationSubmissionIds(ExpectedEmpRef)).ReturnsAsync(new List<long> { 1, 2, 3, 4 });
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.PublishAsync(It.IsAny<ProcessDeclarationsEvent>()), Times.Never);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsAnEndOfYearAdjustmentTheFlagIsSetOnTheRecord()
        {
            //Arrange
            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>())).Returns(true);
            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef, "16-17", 12)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 20 });
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Any(d => d.EndOfYearAdjustment)), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsAnEndOfYearAdjustmentTheAmountIsWorkedOutFromThePreviousTaxYearValue()
        {
            //Arrange
            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>())).Returns(true);
            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef, "16-17", 12)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 20 });
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.First().EndOfYearAdjustment && c.First().EndOfYearAdjustmentAmount.Equals(10m)), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsForATaxMonthInTheFutureItWillNotBeProcessed()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithFutureSubmissions(ExpectedEmpRef, DateTime.UtcNow, ExpectedAccountId);
            var declaration = data.EmployerLevyData.Last().Declarations.Declarations.Last();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, It.IsAny<DateTime>())).Returns(true);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Count() == 4), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfLevyDataHasChangedThenALevyDeclarationUpdatedEventIsPublished()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithMultiplePeriods(ExpectedAccountId, DateTime.UtcNow);

            var hashedAccountId = "ABC123";
            _hashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(hashedAccountId);

            var expectedLevyEvents = new List<LevyDeclarationUpdatedEvent> { new LevyDeclarationUpdatedEvent { ResourceUri = "ABC" }, new LevyDeclarationUpdatedEvent { ResourceUri = "ZZZ" } };
            var expectedGenericEvents = new List<GenericEvent> { new GenericEvent { Id = 1 }, new GenericEvent { Id = 2 } };

            _levyEventFactory.Setup(
                x =>
                    x.CreateDeclarationUpdatedEvent(hashedAccountId, data.EmployerLevyData.First().Declarations.Declarations[0].PayrollYear,
                        data.EmployerLevyData.First().Declarations.Declarations[0].PayrollMonth)).Returns(expectedLevyEvents[0]);
            _genericEventFactory.Setup(x => x.Create(expectedLevyEvents[0])).Returns(expectedGenericEvents[0]);
            _levyEventFactory.Setup(
                x =>
                    x.CreateDeclarationUpdatedEvent(hashedAccountId, data.EmployerLevyData.ElementAt(1).Declarations.Declarations[0].PayrollYear,
                        data.EmployerLevyData.ElementAt(1).Declarations.Declarations[0].PayrollMonth)).Returns(expectedLevyEvents[1]);
            _genericEventFactory.Setup(x => x.Create(expectedLevyEvents[1])).Returns(expectedGenericEvents[1]);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<PublishGenericEventCommand>()), Times.Exactly(2));
            _mediator.Verify(x => x.SendAsync(It.Is<PublishGenericEventCommand>(y => y.Event == expectedGenericEvents[0])), Times.Exactly(1));
            _mediator.Verify(x => x.SendAsync(It.Is<PublishGenericEventCommand>(y => y.Event == expectedGenericEvents[1])), Times.Exactly(1));
        }

        [Test]
        public async Task ThenIfThePayrollYearPreDatesTheLevyItIsNotProcessed()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithFutureSubmissions(ExpectedEmpRef, DateTime.Now);
            _hmrcDateService.Setup(x => x.DoesSubmissionPreDateLevy(It.IsAny<string>())).Returns(true);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.IsAny<IEnumerable<DasDeclaration>>(), It.IsAny<string>(), It.IsAny<long>()), Times.Never);
        }

        [Test]
        public async Task ThenShouldWorkOutEndOfYearAdjustmentFromLastLevySubmission()
        {
            //Arrange
            var adjustmentValue = 5;
            var latestDeclaration = new DasDeclaration { LevyDueYtd = 20 };


            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>()))
                            .Returns(true);

            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef, "16-17", 8))
                           .ReturnsAsync(latestDeclaration);

            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId, latestDeclaration.LevyDueYtd - adjustmentValue);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Any(d => d.EndOfYearAdjustment && d.EndOfYearAdjustmentAmount.Equals(adjustmentValue))), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenShouldUserAdjustYtdValueIfNoLevyDeclared()
        {
            //Arrange
            var adjustmentLevyYtd = 8;


            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>()))
                .Returns(true);

            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId, adjustmentLevyYtd);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclarations(It.Is<IEnumerable<DasDeclaration>>(c => c.Any(d => d.EndOfYearAdjustment && d.EndOfYearAdjustmentAmount.Equals(adjustmentLevyYtd))), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public void ThenShouldThrowErrorIfAdjustmentLevyYtdIsNull()
        {
            //Arrange
            var latestDeclaration = new DasDeclaration { LevyDueYtd = 20 };


            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>()))
                .Returns(true);

            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef, "16-17", 8))
                .ReturnsAsync(latestDeclaration);

            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId);

            data.EmployerLevyData.First().Declarations.Declarations.First().LevyDueYtd = null;

            //Act
            Func<Task> action = () => _refreshEmployerLevyDataCommandHandler.Handle(data);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}
