using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;

namespace SFA.DAS.UnitTests.Authorization.Mvc
{
    [TestFixture]
    public class MessageModelBinderTests
    {
        private MessageModelBinder _modelBinder;
        private ControllerContext _controllerContext;
        private ModelBindingContext _bindingContext;
        private NameValueCollectionValueProvider _valueProvider;
        private Mock<ICallerContextProvider> _callerContextProvider;
        private CallerContext _callerContext;

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

            _callerContextProvider = new Mock<ICallerContextProvider>();

            _callerContext = new CallerContext
            {
                AccountHashedId = "ABC123",
                AccountId = 111111,
                UserRef = Guid.NewGuid()
            };

            _callerContextProvider.Setup(p => p.GetCallerContext()).Returns(_callerContext);

            _modelBinder = new MessageModelBinder(() => _callerContextProvider.Object);
        }

        [Test]
        public void WhenIBindAModelThenShouldBindMembershipMessageProperties()
        {
            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MembershipMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountHashedId, Is.EqualTo(_callerContext.AccountHashedId));
            Assert.That(message.AccountId, Is.EqualTo(_callerContext.AccountId));
            Assert.That(message.UserRef, Is.EqualTo(_callerContext.UserRef));
        }

        [Test]
        public void WhenIBindAModelThenShouldNotBindMessagePropertiesIfContextIsEmpty()
        {
            _callerContext.AccountHashedId = null;
            _callerContext.AccountId = null;
            _callerContext.UserRef = null;

            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MembershipMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountHashedId, Is.Null);
            Assert.That(message.AccountId, Is.Null);
            Assert.That(message.UserRef, Is.Null);
        }

        [Test]
        public void WhenIBindAModelThenShouldNotBindMessagePropertiesForUnknownMessageType()
        {
            _bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MessageStub));

            var message = _modelBinder.BindModel(_controllerContext, _bindingContext) as MessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountHashedId, Is.Null);
            Assert.That(message.AccountId, Is.Null);
            Assert.That(message.UserId, Is.Null);
        }

        private class MembershipMessageStub : MembershipMessage
        {
        }

        private class MessageStub
        {
            public string AccountHashedId { get; set; }
            public long? AccountId { get; set; }
            public Guid? UserId { get; set; }
        }
    }
}