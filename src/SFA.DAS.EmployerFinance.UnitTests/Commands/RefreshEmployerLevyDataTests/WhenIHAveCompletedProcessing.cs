using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Factories;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.EmployerFinance.Models.HmrcLevy;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Testing;
using SFA.DAS.Testing;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RefreshEmployerLevyDataTests
{
    [TestFixture]
    public class RefreshEmployerLevyDataCommandHandlerTests : FluentTest<RefreshEmployerLevyDataCommandHandlerTestsFixture>
    {
        [Test]
        public Task WhenIHaveCompletedProcessing_AndHaveNewLevy()
        {
            const long accountId = 999;
            const string empRef = "ABC/12345";

            return RunAsync(f => f
                    .SetAccountId(accountId)
                    .SetIsSubmissionForFuturePeriod(false)
                    .SetLastSubmissionForScheme(empRef, new DasDeclaration { LevyDueYtd = 1000m, LevyAllowanceForFullYear = 1200m })
                    .SetEmployerLevyData(new List<EmployerLevyData>{new EmployerLevyData
                    {
                        EmpRef = empRef,
                        Declarations = new DasDeclarations{
                            Declarations = new List<DasDeclaration>{
                                new DasDeclaration{
                                    LevyDueYtd = 2000,
                                    PayrollYear = "2018",
                                    PayrollMonth = 6
                                }
                            }
                        }
                    }})
                , f => f.Handle(), (f, r) => { f.VerifyRefreshEmployerLevyDataCompletedEventIsPublished(true); });
        }
    }

    public class RefreshEmployerLevyDataCommandHandlerTestsFixture : FluentTestFixture
    {
        private readonly TestableEventPublisher _eventPublisher;
        private readonly Mock<IDasLevyRepository> _dasLevyRepository;
        private readonly Mock<IHmrcDateService> _hmrcDateService;
        private readonly RefreshEmployerLevyDataCommandHandler _handler;


        private long _accountId = 999;
        private ICollection<EmployerLevyData> _employerLevyData;

        public RefreshEmployerLevyDataCommandHandlerTestsFixture()
        {
            _dasLevyRepository = new Mock<IDasLevyRepository>();
            var genericEventFactory = new Mock<IGenericEventFactory>();
            var hashingService = new Mock<IHashingService>();
            var levyEventFactory = new Mock<ILevyEventFactory>();
            var logger = new Mock<ILog>();
            var mediator = new Mock<IMediator>();
            _eventPublisher = new TestableEventPublisher();

            var validator = new Mock<IValidator<RefreshEmployerLevyDataCommand>>();
            validator.Setup(x => x.Validate(It.IsAny<RefreshEmployerLevyDataCommand>())).Returns(new ValidationResult());

            _hmrcDateService = new Mock<IHmrcDateService>();

            var levyImportCleanerStrategy = new LevyImportCleanerStrategy(_dasLevyRepository.Object, _hmrcDateService.Object, logger.Object);

            _handler = new RefreshEmployerLevyDataCommandHandler(validator.Object, _dasLevyRepository.Object, mediator.Object,
                levyEventFactory.Object, genericEventFactory.Object, hashingService.Object, levyImportCleanerStrategy, _eventPublisher);
        }

        public RefreshEmployerLevyDataCommandHandlerTestsFixture SetAccountId(long accountId)
        {
            _accountId = accountId;

            return this;
        }

        public RefreshEmployerLevyDataCommandHandlerTestsFixture SetIsSubmissionForFuturePeriod(bool result)
        {
            _hmrcDateService.Setup(x => x.IsSubmissionForFuturePeriod(It.IsAny<string>(), It.IsAny<short>(), It.IsAny<DateTime>())).Returns(result);

            return this;
        }

        public RefreshEmployerLevyDataCommandHandlerTestsFixture SetLastSubmissionForScheme(string empRef, DasDeclaration lastDeclaration)
        {
            _dasLevyRepository.Setup(x => x.GetLastSubmissionForScheme(empRef)).ReturnsAsync(lastDeclaration);

            return this;
        }

        public RefreshEmployerLevyDataCommandHandlerTestsFixture SetEmployerLevyData(ICollection<EmployerLevyData> employerLevyData)
        {
            _employerLevyData = employerLevyData;

            return this;
        }
        public Task<Unit> Handle()
        {
            return _handler.Handle(new RefreshEmployerLevyDataCommand()
            {
                AccountId = _accountId,
                EmployerLevyData = _employerLevyData
            });
        }

        public void VerifyRefreshEmployerLevyDataCompletedEventIsPublished(bool expectedLevyImportedValue)
        {
            Assert.IsTrue(_eventPublisher.Events.OfType<RefreshEmployerLevyDataCompletedEvent>().Any(e =>
                e.AccountId.Equals(_accountId)
                && e.LevyImported.Equals(expectedLevyImportedValue)));
        }
    }
}
