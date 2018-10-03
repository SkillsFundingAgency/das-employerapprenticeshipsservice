using System.Threading.Tasks;
using BoDi;
using SFA.DAS.EmployerFinance.AcceptanceTests.Extensions;
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
        public async Task GivenWeHaveAnAccount()
        {
            await _objectContext.CreateAccount(_objectContainer);
        }
    }
}