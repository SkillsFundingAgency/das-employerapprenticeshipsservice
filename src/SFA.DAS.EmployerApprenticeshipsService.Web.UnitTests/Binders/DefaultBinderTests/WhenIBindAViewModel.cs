using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Web.Authorization;
using SFA.DAS.EAS.Web.Binders;

namespace SFA.DAS.EAS.Web.UnitTests.Binders.DefaultBinderTests
{
    [TestFixture]
    public class WhenIBindAViewModel
    {
        private DefaultBinder _binder;
        private ControllerContext _controllerContext;
        private ModelBindingContext _bindingContext;
        private NameValueCollectionValueProvider _valueProvider;
        private Mock<IMembershipService> _membershipService;
        private IMembershipContext _membershipContext;

        [SetUp]
        public void Arrange()
        {
            _controllerContext = new ControllerContext(Mock.Of<HttpContextBase>(), new RouteData(), Mock.Of<ControllerBase>());
            _valueProvider = new NameValueCollectionValueProvider(new NameValueCollection(), null);

            _bindingContext = new ModelBindingContext
            {
                ModelName = "",
                ValueProvider = _valueProvider,
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(AuthorizedMessageStub))
            };

            _membershipService = new Mock<IMembershipService>();

            _membershipContext = new MembershipContext
            {
                AccountHashedId = "ABC123",
                AccountId = 111111,
                UserExternalId = Guid.NewGuid(),
                UserId = 222222
            };

            _membershipService.Setup(c => c.GetMembershipContext()).Returns(_membershipContext);

            _binder = new DefaultBinder(() => _membershipService.Object);
        }

        [Test]
        public void ThenShouldBindAuthorizedMessageProperties()
        {
            var message = _binder.BindModel(_controllerContext, _bindingContext) as AuthorizedMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountHashedId, Is.EqualTo(_membershipContext.AccountHashedId));
            Assert.That(message.AccountId, Is.EqualTo(_membershipContext.AccountId));
            Assert.That(message.UserId, Is.EqualTo(_membershipContext.UserId));
        }

        [Test]
        public void ThenShouldNotBindAuthorizedMessagePropertiesIfMembershipIsNull()
        {
            _membershipService.Setup(m => m.GetMembershipContext()).Returns<Membership>(null);

            var message = _binder.BindModel(_controllerContext, _bindingContext) as AuthorizedMessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountId, Is.Null);
            Assert.That(message.UserId, Is.Null);
        }

        [Test]
        public void ThenShouldNotBindAuthorizedMessagePropertiesForNonAuthorizedMessageType()
        {
            _bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MessageStub));

            var message = _binder.BindModel(_controllerContext, _bindingContext) as MessageStub;

            Assert.That(message, Is.Not.Null);
            Assert.That(message.AccountHashedId, Is.Null);
            Assert.That(message.AccountId, Is.Null);
            Assert.That(message.UserId, Is.Null);
        }

        private class AuthorizedMessageStub : AuthorizedMessage
        {
        }

        private class MessageStub
        {
            public string AccountHashedId { get; set; }
            public long? AccountId { get; set; }
            public long? UserId { get; set; }
        }
    }
}