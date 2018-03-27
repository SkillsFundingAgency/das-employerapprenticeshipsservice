using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMoq;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types.Events.Levy;
using SFA.DAS.EAS.Application.Commands.PublishGenericEvent;
using SFA.DAS.EAS.Application.Commands.RefreshEmployerLevyData;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.TestCommon.ObjectMothers;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.HashingService;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    public class WhenIReceiveTheCommand
    {
        private AutoMoqer _moqer;
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
            _moqer = new AutoMoqer();

            _levyRepository = _moqer.GetMock<IDasLevyRepository>();
            _levyRepository.Setup(x => x.GetLastSubmissionForScheme(ExpectedEmpRef)).ReturnsAsync(new DasDeclaration { LevyDueYtd = 1000m, LevyAllowanceForFullYear = 1200m });

            _validator = _moqer.GetMock<IValidator<RefreshEmployerLevyDataCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());

            _mediator = _moqer.GetMock<IMediator>();

            _hmrcDateService = _moqer.GetMock<IHmrcDateService>();
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<DateTime>())).Returns(false);

            _levyEventFactory = _moqer.GetMock<ILevyEventFactory>();
            _genericEventFactory = _moqer.GetMock<IGenericEventFactory>();
            _hashingService = _moqer.GetMock<IHashingService>();


            _refreshEmployerLevyDataCommandHandler = _moqer.Resolve<RefreshEmployerLevyDataCommandHandler>();
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
        public async Task ThenIfLevyDataHasChangedThenAGenericLevyDeclarationUpdatedEventIsPublished()
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
        public async Task ThenIfLevyDataHasChangedThenALevySchemeDeclarationUpdatedEventIsPublished()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithMultiplePeriods(ExpectedAccountId, DateTime.UtcNow);

            var hashedAccountId = "ABC123";
            _hashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(hashedAccountId);
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(1);

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

            var oldTaxYear = DateTime.Today.Month < 4;
            var payrollMonth = (short)(oldTaxYear ? 9 + DateTime.Today.Month : DateTime.Today.Month - 3);
            var year = DateTime.Today.AddYears(oldTaxYear ? -1 : 0);
            var payrollYear = $"{year:yy}-{year.AddYears(1):yy}";
            //TODO: should really be linked to the data generated by RefreshEmployerLevyDataCommandObjectMother
            _levyRepository
                .Setup(x =>
                    x.GetAccountLevyDeclarations(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<short>()))
                .ReturnsAsync(new List<LevyDeclarationView>
                {
                    new LevyDeclarationView
                    {
                        AccountId = 1,
                        LevyDeclaredInMonth = 100m,
                        PayrollMonth = payrollMonth,
                        PayrollYear = payrollYear,
                        CreatedDate = DateTime.UtcNow,
                        LevyDueYtd = 1000m,
                        LevyAllowanceForYear = 12000m,
                        EmpRef = "abcd"
                    },
                    new LevyDeclarationView
                    {
                        AccountId = 1,
                        LevyDeclaredInMonth = 300m,
                        PayrollMonth = payrollMonth,
                        PayrollYear = payrollYear,
                        CreatedDate = DateTime.UtcNow,
                        LevyDueYtd = 1000m,
                        LevyAllowanceForYear = 12000m,
                        EmpRef = "efgh"
                    }
                });

            _moqer.GetMock<IMapper>()
                .Setup(x => x.Map(It.IsAny<LevyDeclarationView>(), It.IsAny<LevyDeclarationProcessedEvent>()))
                .Callback<LevyDeclarationView, LevyDeclarationProcessedEvent>((view, msg) =>
                {
                    msg.LevyDeclaredInMonth = view.LevyDeclaredInMonth;
                    msg.EmpRef = view.EmpRef;
                });

            //Act
            await _refreshEmployerLevyDataCommandHandler.Handle(data);

            //Assert
            var publisher = _moqer.GetMock<IMessagePublisher>();
            publisher.Verify(x => x.PublishAsync(It.Is<LevyDeclarationProcessedEvent>(msg => msg.AccountId == ExpectedAccountId && msg.LevyDeclaredInMonth == 100 && msg.EmpRef == "abcd")), Times.Exactly(2));
            publisher.Verify(x => x.PublishAsync(It.Is<LevyDeclarationProcessedEvent>(msg => msg.AccountId == ExpectedAccountId && msg.LevyDeclaredInMonth == 300 && msg.EmpRef == "efgh")), Times.Exactly(2));
        }

        [Test]
        public async Task ThenIfLevyDataHasChangedThenALevyDeclarationsProcessedEventIsPublishedForEachPeriodChanged()
        {
            //Arrange
            var data = RefreshEmployerLevyDataCommandObjectMother.CreateLevyDataWithMultiplePeriods(ExpectedAccountId, DateTime.UtcNow);

            var hashedAccountId = "ABC123";
            _hashingService.Setup(x => x.HashValue(ExpectedAccountId)).Returns(hashedAccountId);
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(1);

            var expectedLevyEvents = new List<LevyDeclarationUpdatedEvent> { new LevyDeclarationUpdatedEvent { ResourceUri = "ABC" }, new LevyDeclarationUpdatedEvent { ResourceUri = "ZZZ" } };
            var expectedGenericEvents = new List<GenericEvent> { new GenericEvent { Id = 1 }, new GenericEvent { Id = 2 } };
            //TODO: should really be linked to the data generated by RefreshEmployerLevyDataCommandObjectMother
            var oldTaxYear = DateTime.Today.Month < 4;
            var payrollMonth = (short)(oldTaxYear ? 9 + DateTime.Today.Month : DateTime.Today.Month - 3);
            var year = DateTime.Today.AddYears(oldTaxYear ? -1 : 0);
            var payrollYear = $"{year:yy}-{year.AddYears(1):yy}";
            _levyRepository.Setup(x =>
                    x.GetAccountLevyDeclarations(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<short>()))
                .Returns(Task.FromResult<List<LevyDeclarationView>>(new List<LevyDeclarationView>
                {
                    new LevyDeclarationView
                    {
                        AccountId = 1,
                        LevyDeclaredInMonth = 960m,
                        PayrollMonth = payrollMonth,
                        PayrollYear = payrollYear,
                        CreatedDate = DateTime.UtcNow,
                        LevyDueYtd = 1000m,
                        LevyAllowanceForYear = 12000m,
                        EmpRef = "abcd"
                    },
                    new LevyDeclarationView
                    {
                        AccountId = 1,
                        LevyDeclaredInMonth = 960m,
                        PayrollMonth = payrollMonth,
                        PayrollYear = payrollYear,
                        CreatedDate = DateTime.UtcNow,
                        LevyDueYtd = 1000m,
                        LevyAllowanceForYear = 12000m,
                        EmpRef = "efgh"
                    }
                }));

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
            await _moqer.Resolve<RefreshEmployerLevyDataCommandHandler>().Handle(data);

            //Assert
            var publisher = _moqer.GetMock<IMessagePublisher>();
            publisher.Verify(x => x.PublishAsync(It.Is<LevyDeclarationsProcessedEvent>(msg => msg.LevyDeclaredInMonth == 1920m && msg.AccountId == ExpectedAccountId)), Times.Exactly(2));
        }

    }
}
