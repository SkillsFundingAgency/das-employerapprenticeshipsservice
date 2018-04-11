using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Binders;
using AuthorizationContext = SFA.DAS.EAS.Domain.Models.Authorization.AuthorizationContext;

namespace SFA.DAS.EAS.Web.UnitTests.Binders.DefaultBinderTests
{
    [TestFixture]
    public class WhenIBindAViewModel
    {
        private MessageModelBinder _modelBinder;
        private ControllerContext _controllerContext;
        private ModelBindingContext _bindingContext;
        private NameValueCollectionValueProvider _valueProvider;
        private Mock<IAuthorizationService> _authorizationService;
        private AuthorizationContext _authorizationContext;

        [SetUp]
        public void Arrange()
        {
            _controllerContext = new ControllerContext(Mock.Of<HttpContextBase>(), new RouteData(), Mock.Of<ControllerBase>());
            _valueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), null);

            _bindingContext = new ModelBindingContext
            {
                ModelName = "",
                ValueProvider = _valueProvider,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MembershipMessageStub))
            };

            _authorizationService = new Mock<IAuthorizationService>();

            _authorizationContext = new AuthorizationContext
            {
                AccountContext = new AccountContext {  PublicHashedId = "ABC123", Id = 111111 },
                UserContext = new UserContext { ExternalId = Guid.NewGuid(), Id = 222222 },
            };

            _authorizationService.Setup(c => c.GetAuthorizationContext()).Returns(_authorizationContext);

            _modelBinder = new MessageModelBinder(() => _authorizationService.Object);
        }

        [Test]
        public void ThenShouldBindAuthorizedMessageProperties()
        {
            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MembershipMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountId, Is.EqualTo(_authorizationContext.AccountContext.Id));
            Assert.That(message.UserId, Is.EqualTo(_authorizationContext.UserContext.Id));
        }

        [Test]
        public void ThenShouldNotBindAccountMessagePropertiesIfAccountContextIsNull()
        {
            _authorizationContext.AccountContext = null;

            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MembershipMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountId, Is.Null);
        }

        [Test]
        public void ThenShouldNotBindUserMessagePropertiesIfUserContextIsNull()
        {
            _authorizationContext.UserContext = null;

            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MembershipMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.UserId, Is.Null);
        }

        [Test]
        public void ThenShouldNotBindAuthorizedMessagePropertiesForUnknownMessageType()
        {
            _bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MessageStub));

            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountId, Is.Null);
            Assert.That(message.UserId, Is.Null);
        }

        private class MembershipMessageStub : MembershipMessage
        {
        }

        private class MessageStub
        {
            public long? AccountId { get; set; }
            public long? UserId { get; set; }
        }
    }
}