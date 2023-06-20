using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Controllers.LegalEntitiesControllerTests
{
    public abstract class LegalEntitiesControllerTests
    {
        protected LegalEntitiesController Controller;
        protected Mock<IMediator> Mediator;
        protected Mock<IUrlHelper> UrlTestHelper;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            UrlTestHelper = new Mock<IUrlHelper>();

            Controller = new LegalEntitiesController(Mediator.Object);

            Controller.Url = UrlTestHelper.Object;

        }
    }
}
