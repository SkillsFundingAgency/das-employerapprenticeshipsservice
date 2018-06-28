using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.EntityFramework.WebApi;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Filters.UnitOfWorkManagerFilterTests
{
    [TestFixture]
    public class WhenIRequestAResponse
    {
        private UnitOfWorkManagerFilter _filter;
        private HttpActionExecutedContext _actionExecutedContext;
        private HttpActionContext _actionContext;
        private HttpControllerContext _controllerContext;
        private HttpRequestMessage _httpRequestMessage;
        private Mock<IDependencyScope> _dependencyScope;
        private Mock<IContainer> _container;
        private Mock<IUnitOfWorkManager> _unitOfWorkManager;

        [SetUp]
        public void Arrange()
        {
            _httpRequestMessage = new HttpRequestMessage();
            _controllerContext = new HttpControllerContext { Request = _httpRequestMessage };
            _actionContext = new HttpActionContext { ControllerContext = _controllerContext };
            _actionExecutedContext = new HttpActionExecutedContext(_actionContext, null) { Response = new HttpResponseMessage() };
            _dependencyScope = new Mock<IDependencyScope>();
            _container = new Mock<IContainer>();
            _unitOfWorkManager = new Mock<IUnitOfWorkManager>();
            
            _container.Setup(c => c.GetInstance<IUnitOfWorkManager>(It.IsAny<ExplicitArguments>())).Returns(_unitOfWorkManager.Object);
            _dependencyScope.Setup(s => s.GetService(typeof(IContainer))).Returns(_container.Object);
            _httpRequestMessage.Properties[HttpPropertyKeys.DependencyScope] = _dependencyScope.Object;

            _filter = new UnitOfWorkManagerFilter();
        }

        [Test]
        public void ThenShouldEndTheUnitOfWork()
        {
            _filter.OnActionExecuted(_actionExecutedContext);

            _unitOfWorkManager.Verify(m => m.End(null), Times.Once);
        }

        [Test]
        public void ThenShouldNotEndTheUnitOfWorkIfThereWasAnException()
        {
            _actionExecutedContext.Exception = new Exception();

            _filter.OnActionExecuted(_actionExecutedContext);

            _unitOfWorkManager.Verify(m => m.End(null), Times.Never);
        }
    }
}