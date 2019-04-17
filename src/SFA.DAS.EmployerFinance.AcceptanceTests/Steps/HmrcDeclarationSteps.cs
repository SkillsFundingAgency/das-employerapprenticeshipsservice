using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Moq;
using NServiceBus;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.NLog.Logger;
using TechTalk.SpecFlow;
using SFA.DAS.EmployerFinance.Interfaces;


namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class HmrcDeclarationSteps : TechTalk.SpecFlow.Steps
    {
        public const int StepTimeout = 2 * 60 * 1000;   // 1 minute

        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        //scenario context

        public HmrcDeclarationSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"Hmrc return the following submissions for paye scheme")]
        public void GivenIHaveTheFollowingSubmissionsForEmpRef(Table table)
        {
            var empRef = _objectContext.Get<string>(Extensions.Constants.ObjectContextKeys.EmpRef);
            SetupLastEnglishFractionUpdateDate();
            SetupEnglishFractions(empRef, table);
            SetupLevyDeclarations(empRef, table);
        }

        [When(@"we refresh levy data for paye scheme on the (.*)/(.*)")]
        public Task WhenWeRefreshLevyDataOnGivenDate(int month, int year)
        {
            var currentDateTime = _objectContainer.Resolve<Mock<ICurrentDateTime>>();
            currentDateTime.Setup(x => x.Now).Returns(new DateTime(year, month, 23));

            return WhenWeRefreshLevyData();
        }
        [Given(@"we refresh levy data for paye scheme on the (.*)/(.*)")]
        public Task WhenWeRefreshLevyDataOnGivenDateGiven(int month, int year)
        {
            var currentDateTime = _objectContainer.Resolve<Mock<ICurrentDateTime>>();
            currentDateTime.Setup(x => x.Now).Returns(new DateTime(year, month, 23));

            return WhenWeRefreshLevyData();
        }

        [When(@"we refresh levy data for paye scheme")]
        public Task WhenWeRefreshLevyData()
        {
            var timeout = Debugger.IsAttached ? 10 * 60 * 1000 : StepTimeout;
            var cancellationTokenSource = new CancellationTokenSource(timeout);

            var account = _objectContext.Get<Account>();

            _objectContainer.Resolve<ILog>().Info("About to start levy run task.");

            return _objectContainer.RunStepsInIsolation(cancellationTokenSource.Token,
                    // step 1: send request to get levy declarations to start that process running...
                    c =>
                    {
                        _objectContainer.Resolve<ILog>().Info("About to start levy run.");

                        var empref = _objectContext.GetEmpRef();

                        return c.Resolve<IMessageSession>().Send(new ImportAccountLevyDeclarationsCommand
                        {
                            AccountId = account.Id,
                            PayeRef = empref
                        });
                    },

                    // step 2: wait for the levy declaration process to finish writing the transactions...
                    async c => 
                    {
                        _objectContainer.Resolve<ILog>().Info("About to start polling for levy decalaration.");
                        var repo = c.Resolve<ITransactionRepository>();

                        var allLevyDeclarationsLoaded = await c.Resolve<ITransactionRepository>()
                            .WaitForAllTransactionLinesInDatabase(account, cancellationTokenSource.Token);

                        if (!allLevyDeclarationsLoaded)
                        {
                            throw new Exception($"The levy declarations have not been completely loaded within the allowed time ({timeout} msecs). Either they are still loading or something has failed.");
                        }
                    });
        }


        [When(@"all the transaction lines in this scenario have had their transaction date updated to their created date")]
        public Task WhenScenarioTransactionLinesTransactionDateHaveBeenUpdatedToTheirCreatedDate()
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var transactionRepository = c.Resolve<ITestTransactionRepository>();

                await transactionRepository.SetTransactionLineDateCreatedToTransactionDate(_objectContext
                    .ProcessingSubmissionIds());
            });
        }

        [When(@"all the transaction lines in this scenario have had their transaction date updated to the specified created date")]
        public Task WhenAllTheTransactionLinesInThisScenarioHaveHadTheirTransactionDateUpdatedToTheSpecifiedCreatedDate()
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var transactionRepository = c.Resolve<ITestTransactionRepository>();

                await transactionRepository.SetTransactionLineDateCreatedToTransactionDate(_objectContext
                    .ProcessingSubmissionIdsDictionary());
            });
        }

        private void SetupLastEnglishFractionUpdateDate()
        {
            _objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>()
                .Setup(x => x.GetLastEnglishFractionUpdate(It.IsAny<string>()))
                .ReturnsAsync(DateTime.MinValue);
        }

        private void SetupEnglishFractions(string empRef, Table table)
        {
            var fractionCalculations = new List<FractionCalculation>();
            fractionCalculations.ImportFractionCalculations(table);

            _objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>()
                .Setup(x =>
                    x.GetEmployerFractionCalculations(It.IsAny<string>(), It.Is<string>(s => s.Equals(empRef)), null, null))
                .ReturnsAsync(new EnglishFractionDeclarations
                {
                    Empref = empRef,
                    FractionCalculations = fractionCalculations
                });
        }

        private void SetupLevyDeclarations(string empRef, Table table)
        {
            var levyDeclarations = new LevyDeclarations { EmpRef = empRef, Declarations = new List<Declaration>() };
            levyDeclarations.ImportData(table);

            _objectContext.ImportCurrentlyProcessingSubmissionIds(table);

            _objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>()
                .Setup(x => x.GetEmployerLevyDeclarations(It.IsAny<string>(),
                    It.Is<string>(s => s.Equals(empRef)), It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>()))
                .ReturnsAsync(levyDeclarations);
        }
    }
}