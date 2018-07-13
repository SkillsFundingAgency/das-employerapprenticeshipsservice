﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web.Filters;

namespace SFA.DAS.EAS.Web.UnitTests.Filters.UnitOfWorkManagerFilterTests
{
    [TestFixture]
    public class WhenIViewAPage
    {
        private UnitOfWorkManagerFilter _filter;
        private ActionExecutedContext _filterContext;
        private RouteData _routeData;
        private Mock<IUnitOfWorkManagerAccount> _unitOfWorkManager;

        [SetUp]
        public void Arrange()
        {
            _unitOfWorkManager = new Mock<IUnitOfWorkManagerAccount>();
            _routeData = new RouteData();

            _filterContext = new ActionExecutedContext
            {
                RouteData = _routeData
            };

            _filter = new UnitOfWorkManagerFilter(() => _unitOfWorkManager.Object);
        }

        [Test]
        public void ThenShouldEndTheUnitOfWork()
        {
            _filter.OnActionExecuted(_filterContext);

            _unitOfWorkManager.Verify(m => m.End(), Times.Once);
        }

        [Test]
        public void ThenShouldNotEndTheUnitOfWorkIfTheActionIsAChildAction()
        {
            _routeData.DataTokens["ParentActionViewContext"] = "";

            _filter.OnActionExecuted(_filterContext);

            _unitOfWorkManager.Verify(m => m.End(), Times.Never);
        }

        [Test]
        public void ThenShouldNotEndTheUnitOfWorkIfThereWasAnException()
        {
            _filterContext.Exception = new Exception();

            _filter.OnActionExecuted(_filterContext);

            _unitOfWorkManager.Verify(m => m.End(), Times.Never);
        }
    }
}