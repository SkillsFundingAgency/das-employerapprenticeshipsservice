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
using SFA.DAS.EmployerFinance.Models.Paye;
using TechTalk.SpecFlow;


namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class HmrcDeclarationSteps : TechTalk.SpecFlow.Steps
    {
        public const int StepTimeout = 3 * 60 * 1000;

        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

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

        [When(@"we refresh levy data for paye scheme")]
        public Task WhenWeRefreshLevyData()
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                var empref = _objectContext.GetEmpRef();

                var cancellationTokenSource = new CancellationTokenSource(Debugger.IsAttached ? -1 : StepTimeout);
                var account = _objectContext.Get<Account>();

                await c.Resolve<IMessageSession>().Send(new ImportAccountLevyDeclarationsCommand
                {
                    AccountId = account.Id,
                     PayeRef = empref
                });

                var allLevyDeclarationsLoaded = await c.Resolve<ITransactionRepository>()
                    .WaitForAllTransactionLinesInDatabase(account, cancellationTokenSource.Token);

                if (!allLevyDeclarationsLoaded)
                {
                    throw new Exception($"The levy declarations have not been completely loaded within the allowed time ({StepTimeout} msecs). Either they are still loading or something has failed.");
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