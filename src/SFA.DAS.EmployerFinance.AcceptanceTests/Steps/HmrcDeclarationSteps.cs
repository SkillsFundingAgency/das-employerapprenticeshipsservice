using System;
using System.Collections.Generic;
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
        public const int StepTimeout = 10000;

        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;
        private readonly UnitOfWorkManagerTestHelper _unitOfWorkManagerTestHelper;

        public HmrcDeclarationSteps(IObjectContainer objectContainer, ObjectContext objectContext, UnitOfWorkManagerTestHelper unitOfWorkManagerTestHelper)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
            _unitOfWorkManagerTestHelper = unitOfWorkManagerTestHelper;
        }

        [Given(@"Hmrc return the following submissions for paye scheme ([^ ]*)")]
        public void GivenIHaveTheFollowingSubmissionsForEmpRef(string empRef, Table table)
        {
            SetPayeSchemeRef(empRef);
            SetupLastEnglishFractionUpdateDate();
            SetupEnglishFractions(empRef, table);
            SetupLevyDeclarations(empRef, table);
        }

        private Task<TOperationResultType> Run<TOperationResultType>(Func<Task<TOperationResultType>> operation)
        {
            return _unitOfWorkManagerTestHelper.RunInIsolatedTransactionAsync(operation);
        }

        private Task Run(Func<Task> operation)
        {
            return _unitOfWorkManagerTestHelper.RunInIsolatedTransactionAsync(operation);
        }

        [When(@"we refresh levy data for paye scheme ([^ ]*)")]
        public Task WhenWeRefreshLevyData(string payeScheme)
        {
            var account = _objectContext.FirstOrDefault<Account>();

            return Task.WhenAll(
                Run(() => _objectContext.InitiateJobServiceBusEndpoint.Send(new ImportAccountLevyDeclarationsCommand
                {
                    AccountId = account.Id,
                    PayeRef = payeScheme
                })), 

                Run(() => _objectContainer.Resolve<ITransactionRepository>().WaitForTransactionLinesInDatabase(account, StepTimeout)));
        }

        [When(@"all the transaction lines in this scenario have had there transaction date updated to their created date")]
        public async Task WhenScenarioTransactionLinesTransactionDateHaveBeenUpdatedToTheirCreatedDate()
        {
            var transactionRepository = _objectContainer.Resolve<ITestTransactionRepository>();
            await Run(() => transactionRepository.SetTransactionLineDateCreatedToTransactionDate(_objectContext
                .ProcessingSubmissionIds()));
        }

        [When(@"all the transaction lines in this scenario have had there transaction date updated to the specified created date")]
        public void WhenAllTheTransactionLinesInThisScenarioHaveHadThereTransactionDateUpdatedToTheSpecifiedCreatedDate()
        {
            var transactionRepository = _objectContainer.Resolve<ITestTransactionRepository>();
            Run(() => transactionRepository.SetTransactionLineDateCreatedToTransactionDate(_objectContext
                .ProcessingSubmissionIdsDictionary()));
        }

        private void SetPayeSchemeRef(string empRef)
        {
            _objectContainer.Resolve<Mock<IPayeRepository>>().Setup(x => x.GetPayeSchemeByRef(It.IsAny<string>()))
                .ReturnsAsync(new Paye
                {
                    EmpRef = empRef
                    ,RefName = empRef
                });


            _objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>().Setup(x => x.GetEmployerDetails(It.IsAny<string>()))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer()
                    {
                        Name = new Name { EmprefAssociatedName = $"Name{empRef}" }
                    }
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