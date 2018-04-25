﻿using System.Web.Mvc;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Controllers;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.TransferConnectionInvitationsControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransferConnectionInvitationsPage
    {
        private TransferConnectionInvitationsController _controller;

        [SetUp]
        public void Arrange()
        {
            _controller = new TransferConnectionInvitationsController(null, null);
        }

        [Test]
        public void ThenIShouldBeShownTheTransferConnectionInvitationPage()
        {
            var result = _controller.Index() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.Null);
        }
    }
}