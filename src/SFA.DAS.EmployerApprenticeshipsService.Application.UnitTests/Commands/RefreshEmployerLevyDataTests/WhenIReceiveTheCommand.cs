using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Levy;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshEmployerLevyDataTests
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

            _refreshEmployerLevyDataCommandHandler = new RefreshEmployerLevyDataCommandHandler(_validator.Object, _levyRepository.Object, _mediator.Object, _hmrcDateService.Object,
                _levyEventFactory.Object, _genericEventFactory.Object, _hashingService.Object);
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
        public async Task ThenDataIsTakenFromTheDataStoreForTheEmployerRefAndEachDeclarationRow()
        {
            //Arrange
            var refreshEmployerLevyDataCommand = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(refreshEmployerLevyDataCommand);

            //Assert
            _levyRepository.Verify(x => x.GetEmployerDeclaration(It.Is<string>(c => c.Equals("1") || c.Equals("2") || c.Equals("4")), ExpectedEmpRef), Times.Exactly(refreshEmployerLevyDataCommand.EmployerLevyData[0].Declarations.Declarations.Count(c=>!c.NoPaymentForPeriod)));
        }
        
        [Test]
        public async Task ThenTheLevyRepositoryIsUpdatedIfTheDeclarationDoesNotExist()
        {
            //Arrange
            _levyRepository.Setup(x => x.GetEmployerDeclaration("1", ExpectedEmpRef)).ReturnsAsync(null);
            _levyRepository.Setup(x => x.GetEmployerDeclaration("2", ExpectedEmpRef)).ReturnsAsync(new DasDeclaration());

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId));

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclaration(It.IsAny<DasDeclaration>(), ExpectedEmpRef, ExpectedAccountId));
        }
        

        [Test]
        public async Task ThenIfTheDeclarationHasNoSubmissionForPeriodThenTheLAstSubmissionIsReadFromTheRepository()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x=>x.GetLastSubmissionForScheme(ExpectedEmpRef),Times.Exactly(data.EmployerLevyData[0].Declarations.Declarations.Count(c=>c.NoPaymentForPeriod)));
        }

        [Test]
        public async Task ThenIfTheDeclarationHasNoSubmissionThenTheLevyDueYtdIsTakenFromThePreviousSubmission()
        {
            //Act
            var data = RefreshEmployerLevyDataCommandObjectMother.Create(ExpectedEmpRef, ExpectedAccountId);
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x=>x.CreateEmployerDeclaration(It.Is<DasDeclaration>(c=>c.NoPaymentForPeriod && c.LevyDueYtd.Equals(1000) && c.LevyAllowanceForFullYear.Equals(1200m)),ExpectedEmpRef,ExpectedAccountId),Times.Once);
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
            _levyRepository.Setup(x => x.GetEmployerDeclaration(It.IsAny<string>(), ExpectedEmpRef)).ReturnsAsync(new DasDeclaration());
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
            _levyRepository.Verify(x => x.CreateEmployerDeclaration(It.Is<DasDeclaration>(c => c.EndOfYearAdjustment), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsAnEndOfYearAdjustmentTheAmountIsWorkedOutFromThePreviousTaxYearValue()
        {
            //Arrange
            _hmrcDateService.Setup(x => x.IsSubmissionEndOfYearAdjustment("16-17", 12, It.IsAny<DateTime>())).Returns(true);
            _levyRepository.Setup(x => x.GetSubmissionByEmprefPayrollYearAndMonth(ExpectedEmpRef,"16-17",12)).ReturnsAsync(new DasDeclaration {LevyDueYtd = 20});
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateEndOfYearAdjustment(ExpectedEmpRef, ExpectedAccountId);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclaration(It.Is<DasDeclaration>(c => c.EndOfYearAdjustment && c.EndOfYearAdjustmentAmount.Equals(10m)), ExpectedEmpRef, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheSubmissionIsForATaxMonthInTheFutureItWillNotBeProcessed()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithFutureSubmissions(ExpectedEmpRef, DateTime.UtcNow, ExpectedAccountId);
            var declaration = data.EmployerLevyData.Last().Declarations.Declarations.Last();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(declaration.PayrollYear, declaration.PayrollMonth.Value,It.IsAny<DateTime>())).Returns(true);
            
            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _levyRepository.Verify(x => x.CreateEmployerDeclaration(It.IsAny<DasDeclaration>(),ExpectedEmpRef,ExpectedAccountId), Times.Exactly(4));
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
                    x.CreateDeclarationUpdatedEvent(hashedAccountId, data.EmployerLevyData[0].Declarations.Declarations[0].PayrollYear,
                        data.EmployerLevyData[0].Declarations.Declarations[0].PayrollMonth)).Returns(expectedLevyEvents[0]);
            _genericEventFactory.Setup(x => x.Create(expectedLevyEvents[0])).Returns(expectedGenericEvents[0]);
            _levyEventFactory.Setup(
                x =>
                    x.CreateDeclarationUpdatedEvent(hashedAccountId, data.EmployerLevyData[1].Declarations.Declarations[0].PayrollYear,
                        data.EmployerLevyData[1].Declarations.Declarations[0].PayrollMonth)).Returns(expectedLevyEvents[1]);
            _genericEventFactory.Setup(x => x.Create(expectedLevyEvents[1])).Returns(expectedGenericEvents[1]);

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.IsAny<PublishGenericEventCommand>()), Times.Exactly(2));
            _mediator.Verify(x => x.SendAsync(It.Is<PublishGenericEventCommand>(y => y.Event == expectedGenericEvents[0])), Times.Exactly(1));
            _mediator.Verify(x => x.SendAsync(It.Is<PublishGenericEventCommand>(y => y.Event == expectedGenericEvents[1])), Times.Exactly(1));
        }
    }
}
