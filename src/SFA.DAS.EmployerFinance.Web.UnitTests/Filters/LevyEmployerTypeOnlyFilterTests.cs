using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Web.Filters;
using SFA.DAS.EmployerFinance.Web.Helpers;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Filters
{
    public class LevyEmployerTypeOnlyFilterTests
    {
        private Mock<IAccountApiClient> _accountApiClientMock;
        private ActionExecutingContext _filterContext;

        [SetUp]
        public void SetUp()
        {
            _filterContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object> { { "HashedAccountId", "abc123" } }
            };

            _accountApiClientMock = new Mock<IAccountApiClient>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(provider => provider.GetService(typeof(IAccountApiClient)))
                .Returns(_accountApiClientMock.Object);

            var dependencyResolver = new Mock<IDependencyResolver>();
            dependencyResolver
                .Setup(mock => mock.GetService(typeof(IAccountApiClient)))
                .Returns(_accountApiClientMock.Object);

            DependencyResolver.SetResolver(dependencyResolver.Object);
        }

        [Test]
        public void WhenLevyEmployer_ShouldAllowAccess()
        {
            // Arrange
            var sut = new LevyEmployerTypeOnly();
            _accountApiClientMock
                .Setup(mock => mock.GetAccount(It.IsAny<string>()))
                .ReturnsAsync(new AccountDetailViewModel { ApprenticeshipEmployerType = "Levy" });

            // Act
            sut.OnActionExecuting(_filterContext);
            var result = _filterContext.Result as ViewResult;

            // Assert
            _filterContext.Result.Should().BeNull();
        }

        [Test]
        public void WhenNonLevyEmployer_ShouldDenyAccess()
        {
            // Arrange
            var sut = new LevyEmployerTypeOnly();
            _accountApiClientMock
                .Setup(mock => mock.GetAccount(It.IsAny<string>()))
                .ReturnsAsync(new AccountDetailViewModel { ApprenticeshipEmployerType = "NonLevy" });

            // Act
            sut.OnActionExecuting(_filterContext);
            var result = _filterContext.Result as RedirectToRouteResult;

            // Assert
            result.Should().NotBeNull();
            result.RouteValues["controller"].Should().Be("AccessDenied");
        }

        [Test]
        public void WhenGetAccountFails_ShouldRedirectToBadRequest()
        {
            // Arrange
            var sut = new LevyEmployerTypeOnly();
            _accountApiClientMock
                .Setup(mock => mock.GetAccount(It.IsAny<string>()))
                .Throws(new Exception());

            // Act
            sut.OnActionExecuting(_filterContext);
            var result = _filterContext.Result as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be(ControllerConstants.BadRequestViewName);
        }

        [Test]
        public void WhenFilterUsedIncorrectly_ShouldRedirectToBadRequest()
        {
            // Arrange
            var sut = new LevyEmployerTypeOnly();
            _accountApiClientMock
                .Setup(mock => mock.GetAccount(It.IsAny<string>()))
                .Throws(new Exception());

            // Act
            _filterContext = new ActionExecutingContext();
            sut.OnActionExecuting(_filterContext);
            var result = _filterContext.Result as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().Be(ControllerConstants.BadRequestViewName);
        }
    }
}
