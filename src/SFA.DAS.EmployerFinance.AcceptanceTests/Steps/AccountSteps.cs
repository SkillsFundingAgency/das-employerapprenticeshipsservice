using System.Threading.Tasks;
using BoDi;
using HMRC.ESFA.Levy.Api.Client;
using HMRC.ESFA.Levy.Api.Types;
using Moq;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
using SFA.DAS.EmployerFinance.AcceptanceTests.TestRepositories;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Steps
{
    [Binding]
    public class AccountSteps : TechTalk.SpecFlow.Steps
    {
        private readonly IObjectContainer _objectContainer;
        private readonly ObjectContext _objectContext;

        public AccountSteps(IObjectContainer objectContainer, ObjectContext objectContext)
        {
            _objectContainer = objectContainer;
            _objectContext = objectContext;
        }

        [Given(@"We have an account")]
        public Task GivenWeHaveAnAccount()
        {
            return _objectContainer.ScopeAsync(c => _objectContext.CreateAccount(c));
        }

        [Given(@"We have an account with a paye scheme")]
        public Task GivenWeHaveAnAccountWithPayeScheme()
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                await _objectContext.CreateAccount(c);
                await InitialisePayeSchemeRef(c, "123/ACT");
            });
        }

        [Given(@"An employer is adding a PAYE which has submissions older than the (.*) month expiry rule limit")]
        public Task GivenPAYEAccountAndExpiryLimit(int expiryLimit)
        {
            var employerFinanceConfiguration = _objectContainer.Resolve<EmployerFinanceConfiguration>();
            employerFinanceConfiguration.FundsExpiryPeriod = expiryLimit;

            return _objectContainer.ScopeAsync(async c =>
            {
                await _objectContext.CreateAccount(c);
                await InitialisePayeSchemeRef(c, "123/ACT");
            });
        }

        [Given(@"Another account is opened and associated with the paye scheme")]
        public Task GivenAnotherAccountIsOpenedAndAssociatedWithThePayeScheme()
        {
            return _objectContainer.ScopeAsync(async c =>
            {
                await _objectContext.CreateAccount(c);
            });
        }



        private Task InitialisePayeSchemeRef(IObjectContainer objectContainer, string empRef)
        {
            return ClearDownPayeRefsFromDbAsync(objectContainer, empRef)
                .ContinueWith(t =>
                {
                    SetupMocks(objectContainer, empRef);
                    StoreEmpRefInContext(empRef);
                });
        }

        private Task ClearDownPayeRefsFromDbAsync(IObjectContainer objectContainer, string empRef)
        {
            var repo = objectContainer.Resolve<ITestTransactionRepository>();

            return repo.RemovePayeRef(empRef);
        }

        private void SetupMocks(IObjectContainer objectContainer, string empRef)
        {
            objectContainer.Resolve<Mock<IPayeRepository>>().Setup(x => x.GetPayeSchemeByRef(It.IsAny<string>()))
                .ReturnsAsync(new Paye
                {
                    EmpRef = empRef,
                    RefName = empRef
                });


            objectContainer.Resolve<Mock<IApprenticeshipLevyApiClient>>().Setup(x => x.GetEmployerDetails(It.IsAny<string>()))
                .ReturnsAsync(new EmpRefLevyInformation
                {
                    Employer = new Employer()
                    {
                        Name = new Name { EmprefAssociatedName = $"Name{empRef}" }
                    }
                });
        }

        private void StoreEmpRefInContext(string empRef)
        {
            _objectContext.SetEmpRef(empRef);
        }
    }
}