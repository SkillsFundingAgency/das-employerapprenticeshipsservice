using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Web.Attributes;

namespace SFA.DAS.EAS.Web.UnitTests.Attributes.SignOutAttributeTests
{
    [TestFixture]
    public class WhenActionExecuting
    {
        private Web.Attributes.SignOutAttribute _filter;
        private Mock<IAuthenticationService> _authorizationService;
        private ActionExecutingContext _filterContext;

        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthenticationService>();
            _filterContext = new ActionExecutingContext();

            _filter = new SignOutAttribute (() => _authorizationService.Object);
        }

        [Test]
        public void SignOutUserIsCalled()
        {
            _filter.OnActionExecuting(_filterContext);

            _authorizationService.Verify(a => a.SignOutUser(), Times.Once);
        }
    }
}
