using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Binders;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.UnitTests.Binders.DefaultBinderTests
{
    [TestFixture]
    public class WhenIBindAViewModel
    {
        private const string AccountHashedId = "ABC123";

        private DefaultBinder _binder;
        private ControllerContext _controllerContext;
        private RouteData _routeData;
        private ModelBindingContext _bindingContext;
        private NameValueCollectionValueProvider _valueProvider;
        private Mock<ICurrentUserService> _currentUserService;
        private CurrentUser _currentUser;

        [SetUp]
        public void Arrange()
        {
            _routeData = new RouteData();

            _routeData.Values[ControllerConstants.HashedAccountIdKeyName] = AccountHashedId;

            _controllerContext = new ControllerContext(Mock.Of<HttpContextBase>(), _routeData, Mock.Of<ControllerBase>());
            _valueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), null);

            _bindingContext = new ModelBindingContext
            {
                ModelName = "",
                ValueProvider = _valueProvider,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(AuthorizedMessageStub))
            };

            _currentUserService = new Mock<ICurrentUserService>();
            _currentUser = new CurrentUser { ExternalId = Guid.NewGuid() };

            _currentUserService.Setup(c => c.GetCurrentUser()).Returns(_currentUser);

            _binder = new DefaultBinder(() => _currentUserService.Object);
        }

        [Test]
        public void ThenShouldBindAuthorizedMessageProperties()
        {
            var message = _binder.BindModel(_controllerContext, _bindingContext) as AuthorizedMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.UserExternalId, Is.EqualTo(_currentUser.ExternalId));
            Assert.That(message.AccountHashedId, Is.EqualTo(AccountHashedId));
        }

        [Test]
        public void ThenShouldNotBindAuthorizedMessagePropertiesForNonAuthorizedMessageType()
        {
            _bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MessageStub));

            var message = _binder.BindModel(_controllerContext, _bindingContext) as MessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.UserExternalId, Is.Null);
            Assert.That(message.AccountHashedId, Is.Null);
        }
    }

    public class AuthorizedMessageStub : AuthorizedMessage
    {
    }

    public class MessageStub
    {
        public Guid? UserExternalId { get; set; }
        public string AccountHashedId { get; set; }
    }
}