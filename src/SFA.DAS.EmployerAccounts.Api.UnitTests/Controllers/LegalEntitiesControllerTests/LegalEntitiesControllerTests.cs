using System.Web.Http.Routing;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    public abstract class LegalEntitiesControllerTests
    {
        protected LegalEntitiesController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<UrlHelper> UrlHelper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            UrlHelper = new Mock<UrlHelper>();

            Controller = new LegalEntitiesController(Mediator.Object);

            Controller.Url = UrlHelper.Object;

        }
    }
}
