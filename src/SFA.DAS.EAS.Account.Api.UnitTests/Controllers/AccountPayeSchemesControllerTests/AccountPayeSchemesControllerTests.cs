using System.Web.Http.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    public abstract class AccountPayeSchemesControllerTests
    {
        protected AccountPayeSchemesController Controller;
        protected Mock<UrlHelper> UrlHelper;
        protected Mock<IEmployerAccountsApiService> ApiService;

        [SetUp]
        public void Arrange()
        {
            ApiService = new Mock<IEmployerAccountsApiService>();
            
            Controller = new AccountPayeSchemesController(ApiService.Object);

            UrlHelper = new Mock<UrlHelper>();
            Controller.Url = UrlHelper.Object;
        }
    }
}
